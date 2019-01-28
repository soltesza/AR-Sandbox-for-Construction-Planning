/*
 * Cadyts - Calibration of dynamic traffic simulations
 *
 * Copyright 2009-2016 Gunnar Flötteröd
 * 
 *
 * This file is part of Cadyts.
 *
 * Cadyts is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Cadyts is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Cadyts.  If not, see <http://www.gnu.org/licenses/>.
 *
 * contact: gunnar.floetteroed@abe.kth.se
 *
 */ 
package floetteroed.cadyts.calibrators;

import static java.lang.Math.max;
import static java.lang.Math.min;

import java.io.FileNotFoundException;
import java.io.PrintWriter;
import java.io.Serializable;
import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.LinkedHashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.logging.Logger;

import floetteroed.cadyts.demand.Demand;
import floetteroed.cadyts.demand.Plan;
import floetteroed.cadyts.demand.PlanBuilder;
import floetteroed.cadyts.demand.PlanStep;
import floetteroed.cadyts.measurements.MultiLinkMeasurement;
import floetteroed.cadyts.measurements.SingleLinkMeasurement;
import floetteroed.cadyts.supply.SimResults;
import floetteroed.utilities.DynamicData;
import floetteroed.utilities.Time;
import floetteroed.utilities.math.MathHelpers;
import floetteroed.utilities.math.Vector;

/**
 * 
 * A helper class of the Calibrator that is responsible for most algorithmic
 * issues (whereas the Calibrator deals more with the program sequence control).
 * 
 * @author Gunnar Flötteröd
 * 
 * @param L
 *            the network link type
 * 
 */
class Analyzer<L> implements Serializable {

	// -------------------- CONSTANTS --------------------

	private static final long serialVersionUID = 1L;

	private static final double maxAbsPlanLambda = 15;

	// -------------------- MEMBER VARIABLES --------------------

	private final Demand<L> demand;

	private StatisticsTracker statisticsTracker;

	// SINGLE-LINK MEASUREMENTS

	private final List<SingleLinkMeasurement<L>> allSingleLinkMeas;

	private final List<SingleLinkMeasurement<L>> planListeningSingleLinkMeas;

	private final Map<L, List<SingleLinkMeasurement<L>>> link2meas;

	// MULTI-LINK MEASUREMENTS

	private final Set<L> observedLinks;

	private final List<MultiLinkMeasurement<L>> allMultiLinkMeas;

	private Vector matchList;

	private Vector newMatchList;

	// -------------------- CONSTRUCTION --------------------

	Analyzer(final int startTime_s, final int binSize_s, final int binCnt) {

		this.demand = new Demand<L>(startTime_s, binSize_s, binCnt);
		this.statisticsTracker = new StatisticsTracker(null);

		this.allSingleLinkMeas = new ArrayList<SingleLinkMeasurement<L>>();
		this.planListeningSingleLinkMeas = new ArrayList<SingleLinkMeasurement<L>>();
		this.link2meas = new LinkedHashMap<L, List<SingleLinkMeasurement<L>>>();

		this.observedLinks = new LinkedHashSet<L>();
		this.allMultiLinkMeas = new ArrayList<MultiLinkMeasurement<L>>();
		this.matchList = null;
		this.newMatchList = null;
	}

	// -------------------- SETTERS AND GETTERS -------------------

	void setStatisticsFile(final String statisticsFile) {
		this.statisticsTracker = new StatisticsTracker(statisticsFile);
	}

	String getStatisticsFile() {
		return this.statisticsTracker.getFileName();
	}

	// -------------------- SIMPLE FUNCTIONALITY --------------------

	int getBinSize_s() {
		return this.demand.getBinSize_s();
	}

	void freeze() {
		for (SingleLinkMeasurement<L> meas : this.allSingleLinkMeas) {
			meas.freeze();
		}
		for (MultiLinkMeasurement<L> meas : this.allMultiLinkMeas) {
			meas.freeze();
		}
	}

	// -------------------- MEASUREMENT BOOKKEEPING -------------------

	void addMeasurement(final SingleLinkMeasurement<L> meas) {
		this.allSingleLinkMeas.add(meas);
		this.allocateSingleLinkMeasurement(meas);
	}

	void addMeasurement(final MultiLinkMeasurement<L> meas) {
		this.allMultiLinkMeas.add(meas);
		this.observedLinks.addAll(meas.getObservedLinks());
	}

	// -------------------- SINGLE-LINK-HELPERS --------------------

	private void allocateSingleLinkMeasurement(
			final SingleLinkMeasurement<L> meas) {
		if (meas.isPlanListening()) {
			this.planListeningSingleLinkMeas.add(meas);
		}
		for (L link : meas.getRelevantLinks()) {
			List<SingleLinkMeasurement<L>> measList = this.link2meas.get(link);
			if (measList == null) {
				measList = new ArrayList<SingleLinkMeasurement<L>>();
				this.link2meas.put(link, measList);
			}
			measList.add(meas);
		}
	}

	// -------------------- MULTI-LINK-HELPERS --------------------

	private int[] indicesInMeas(final Plan<L> plan) {
		final int[] result = new int[this.allMultiLinkMeas.size()];
		for (PlanStep<L> step : plan) {
			for (int m = 0; m < this.allMultiLinkMeas.size(); m++) {
				final MultiLinkMeasurement<L> meas = this.allMultiLinkMeas
						.get(m);
				final int indexInMeas = result[m];
				if (indexInMeas < meas.size()
						&& meas.appliesTo(indexInMeas, step)) {
					result[m]++;
				}
			}
		}
		return result;
	}

	private int numberOfMatches(int[] indicesInMeas) {
		int result = 0;
		for (int m = 0; m < this.allMultiLinkMeas.size(); m++) {
			if (indicesInMeas[m] == this.allMultiLinkMeas.get(m).size()) {
				result++;
			}
		}
		return result;
	}

	private List<Integer> matchingMeasurementIndices(final Plan<L> plan) {
		final List<Integer> result = new ArrayList<Integer>();
		final int[] indicesInMeas = this.indicesInMeas(plan);
		for (int m = 0; m < this.allMultiLinkMeas.size(); m++) {
			if (indicesInMeas[m] == this.allMultiLinkMeas.get(m).size()) {
				result.add(m);
			}
		}
		return result;
	}

	// -------------------- ANALYSIS FUNCTIONALITY --------------------

	void notifyPlanChoice(final Plan<L> plan) {
		if (plan == null) {
			return;
		}
		/*
		 * (1) NOTIFY SINGLE-LINK MEASUREMENTS
		 */
		for (SingleLinkMeasurement<L> meas : this.planListeningSingleLinkMeas) {
			meas.notifyPlanChoice(plan);
		}
		/*
		 * (2) NOTIFY MULTI-LINK MEASUREMENTS
		 */
		if (this.allMultiLinkMeas.size() > 0) {
			if (this.newMatchList == null) {
				this.newMatchList = new Vector(this.allMultiLinkMeas.size());
			}
			final List<Integer> matchingMeasIndices = this
					.matchingMeasurementIndices(plan);
			final double oneByMatches = 1.0 / matchingMeasIndices.size();
			for (int m : matchingMeasIndices) {
				this.newMatchList.add(m, oneByMatches);
			}
		}
		/*
		 * (3) UPDATE DEMAND AND STATISTICS
		 */
		for (PlanStep<L> planStep : plan) {
			if (this.link2meas.keySet().contains(planStep.getLink())
					|| (this.observedLinks.contains(planStep.getLink()))) {
				this.demand.add(planStep);
			}
		}
		this.statisticsTracker.registerChoice();
	}

	double calcLinearPlanEffect(final Plan<L> plan) {
		if (plan == null) {
			return 0.0;
		}
		double result = 0;
		/*
		 * (1) SINGLE-LINK MEASUREMENTS
		 */
		for (PlanStep<L> step : plan) {
			final List<SingleLinkMeasurement<L>> measList = this.link2meas
					.get(step.getLink());
			if (measList != null) {
				for (SingleLinkMeasurement<L> meas : measList) {
					final double lambda = meas.getLambda(step);
					this.statisticsTracker.registerLinkLambda(lambda);
					result += lambda;
				}
			}
		}
		/*
		 * (2) MULTI-LINK MEASUREMENTS
		 */
		if (this.allMultiLinkMeas.size() > 0) {
			final int[] indicesInMeas = this.indicesInMeas(plan);
			final double numberOfMatches = this.numberOfMatches(indicesInMeas);
			for (int m = 0; m < this.allMultiLinkMeas.size(); m++) {
				if (indicesInMeas[m] == this.allMultiLinkMeas.get(m).size()) {
					result += this.allMultiLinkMeas.get(m).dll_dMatches()
							/ numberOfMatches;
				}
			}
		}
		/*
		 * (4) POSTPROCESS RESULT
		 */
		result = Math.min(result, maxAbsPlanLambda);
		result = Math.max(result, -maxAbsPlanLambda);
		this.statisticsTracker.registerPlanLambda(result);
		return result;
	}

	void afterNetworkLoading(final SimResults<L> simResults,
			final String flowAnalysisFile) {
		/*
		 * (0) dump flow analysis information
		 */
		if (flowAnalysisFile != null) {
			try {
				final PrintWriter writer = new PrintWriter(flowAnalysisFile);
				writer.println("link\tstart-time\tend-time\tstart-time(sec)\t"
						+ "end-time(sec)\ttype\tsimulated\tmeasured\t"
						+ "standard-deviation\terror\tabsolute-error\t"
						+ "relative-error\trelative-absolute-error");
				for (SingleLinkMeasurement<L> meas : this.allSingleLinkMeas) {
					final L link = meas.getLink();
					final SingleLinkMeasurement.TYPE type = meas.getType();
					final int start_s = meas.getStartTime_s();
					final int end_s = meas.getEndTime_s();
					final double simValue = simResults.getSimValue(link,
							start_s, end_s, type);
					final double measValue = meas.getMeasValue();
					writer.print(link);
					writer.print("\t");
					writer.print(Time.strFromSec(start_s, ':'));
					writer.print("\t");
					writer.print(Time.strFromSec(end_s, ':'));
					writer.print("\t");
					writer.print(start_s);
					writer.print("\t");
					writer.print(end_s);
					writer.print("\t");
					writer.print(type);
					writer.print("\t");
					writer.print(simValue);
					writer.print("\t");
					writer.print(measValue);
					writer.print("\t");
					writer.print(meas.getMeasStddev());
					writer.print("\t");
					writer.print(simValue - measValue);
					writer.print("\t");
					writer.print(Math.abs(simValue - measValue));
					writer.print("\t");
					writer.print((simValue - measValue) / measValue);
					writer.print("\t");
					writer.println(Math.abs(simValue - measValue) / measValue);
				}
				writer.flush();
				writer.close();
			} catch (FileNotFoundException e) {
				e.printStackTrace();
			}
		}

		/*
		 * (1) update regressions
		 * 
		 * All demand data of this iteration was collected conditionally on the
		 * current measurement lists and the current definitions of relevant
		 * links, so the updates should happen before the measurement lists are
		 * rearranged.
		 */
		if (this.allSingleLinkMeas.size() > 0) {
			double ll = 0;
			double llPredErr = 0;
			for (SingleLinkMeasurement<L> meas : this.allSingleLinkMeas) {
				meas.update(this.demand, simResults);
				ll += meas.getLastLL();
				llPredErr += Math.abs(meas.getLastLLPredErr());
			}
			Logger.getLogger(this.getClass().getName()).info(
					"single-link log-likelihood" + " is " + ll + " +/- "
							+ llPredErr);
			this.statisticsTracker.registerSingleLinkLL(ll);
			this.statisticsTracker.registerSingleLinkLLPredError(llPredErr);
		}

		if (this.allMultiLinkMeas.size() > 0) {
			this.matchList = this.newMatchList.copy();
			this.newMatchList = null;
			for (int m = 0; m < this.allMultiLinkMeas.size(); m++) {
				this.allMultiLinkMeas.get(m).update(this.matchList.get(m),
						this.demand, simResults);
			}
			double ll = 0;
			for (int m = 0; m < this.allMultiLinkMeas.size(); m++) {
				ll += this.allMultiLinkMeas.get(m).ll(this.matchList.get(m));
			}
			this.statisticsTracker.registerMultiLinkLL(ll);
		}

		/*
		 * (2) internal updates
		 */
		this.statisticsTracker.writeToFile();
		this.statisticsTracker.clear();
		this.demand.clear();

		/*
		 * (3) (re)allocate measurements to task-specific lists
		 * 
		 * The previous call to Measurement.update(..) might have changed the
		 * internal state of the network. In particular, the plan listening of
		 * the measurements might have changed because their internal subnetwork
		 * representations might have been completed.
		 */
		this.planListeningSingleLinkMeas.clear();
		this.link2meas.clear();
		for (SingleLinkMeasurement<L> meas : this.allSingleLinkMeas) {
			this.allocateSingleLinkMeasurement(meas);
		}
		if (this.planListeningSingleLinkMeas.size() > 0) {
			Logger.getLogger(this.getClass().getName()).info(
					this.planListeningSingleLinkMeas.size()
							+ " inactive measurement(s)");
		}
	}

	// -------------------- TRANSFORMATION INTO DynamicData --------------------

	DynamicData<L> getLinkCostOffsets() {
		Logger.getLogger(this.getClass().getName()).warning(
				"experimental function, "
						+ "accounts only for single-link measurements");
		final DynamicData<L> result = new DynamicData<L>(
				this.demand.getStartTime_s(), this.demand.getBinSize_s(),
				this.demand.getBinCnt());
		for (SingleLinkMeasurement<L> meas : this.allSingleLinkMeas) {
			final int startBin = max(result.bin(meas.getStartTime_s()), 0);
			final int endBin = min(result.bin(meas.getEndTime_s() - 1),
					result.getBinCnt() - 1);
			for (int bin = startBin; bin <= endBin; bin++) {
				final double weight = MathHelpers.overlap(
						result.binStart_s(bin),
						result.binStart_s(bin) + result.getBinSize_s(),
						meas.getStartTime_s(), meas.getEndTime_s())
						/ result.getBinSize_s();
				result.add(meas.getLink(), bin,
						weight * meas.getLambdaCoefficient(meas.getLink()));
			}
		}
		return result;
	}

	// -------------------- REDUCTION OF PLANS --------------------

	// TODO NEW
	Plan<L> newReducedPlan(final Plan<L> fullPlan) {
		final PlanBuilder<L> builder = new PlanBuilder<L>();
		for (PlanStep<L> step : fullPlan) {
			if (this.link2meas.keySet().contains(step.getLink())) {
				builder.addTurn(step.getLink(), step.getEntryTime_s());
			}
		}
		return builder.getResult();
	}

	// TODO NEW
	int getBinCnt() {
		return this.demand.getBinCnt();
	}

	// TODO NEW
	double logLikelihood(final Demand<L> demand) {
		double result = 0;
		for (SingleLinkMeasurement<L> meas : this.allSingleLinkMeas) {
			result += meas.logLikelihood(demand);
		}
		return result;
	}

	// TODO NEW
	double d_logLikelihood_dPlanChoiceProba(final Plan<L> plan,
			final Demand<L> demand) {
		double result = 0;
		for (PlanStep<L> step : plan) {
			for (SingleLinkMeasurement<L> meas : this.link2meas.get(step
					.getLink())) {
				result += meas.d_logLikelihood_d_linkDemand(step.getLink(),
						step.getEntryTime_s(), demand);
			}
		}
		return result;
	}

	// ---------- TODO NEW FOR LEVENBERG-MARCQUARDT ----------

	private Map<SingleLinkMeasurement<L>, Integer> meas2index = null;

	private void createMeasIndex() {
		this.meas2index = new LinkedHashMap<SingleLinkMeasurement<L>, Integer>();
		for (int i = 0; i < this.allSingleLinkMeas.size(); i++) {
			this.meas2index.put(this.allSingleLinkMeas.get(i), i);
		}
		Logger.getLogger(this.getClass().getName())
				.info("Indexed " + this.meas2index.size()
						+ " distinct measurements.");
		if (this.meas2index.size() != this.allSingleLinkMeas.size()) {
			Logger.getLogger(this.getClass().getName()).warning(
					"number of registered single link measurements is "
							+ this.allSingleLinkMeas.size());
		}
	}

	private int measIndex(final SingleLinkMeasurement<L> meas) {
		if (this.meas2index == null) {
			this.createMeasIndex();
		}
		return this.meas2index.get(meas);
	}

	Vector residuals(final Demand<L> demand) {
		final Vector result = new Vector(this.allSingleLinkMeas.size());
		for (SingleLinkMeasurement<L> meas : this.allSingleLinkMeas) {
			result.set(this.measIndex(meas), meas.residual(demand));
		}
		return result;
	}

	Vector dResiduals_dLinkDemand() {
		final Vector result = new Vector(this.allSingleLinkMeas.size());
		for (SingleLinkMeasurement<L> meas : this.allSingleLinkMeas) {
			result.set(this.measIndex(meas), meas.dResidual_dLinkDemand());
		}
		return result;
	}

	List<Integer> affectedMeasurementIndices(final Plan<L> plan) {
		final List<Integer> result = new ArrayList<Integer>();
		for (PlanStep<L> step : plan) {
			for (SingleLinkMeasurement<L> meas : this.link2meas.get(step
					.getLink())) {
				if ((step.getEntryTime_s() >= meas.getStartTime_s())
						&& (step.getEntryTime_s() < meas.getEndTime_s())) {
					result.add(this.measIndex(meas));
				}
			}
		}
		return result;
	}

	// --------------------------------------------------------
	// -------------------- VERY OLD STUFF --------------------
	// --------------------------------------------------------

	// /*
	// * This is new code for the estimation of demand model parameters. It
	// * implements [[TODO COMPLETE]]. Variables are like in that article, only
	// * that "\Pi" is written as "P" and "\beta" as "b".
	// */
	//
	// /**
	// * The Hessian of the sensor data's log-likelihood function L with respect
	// * to the parameter vector b.
	// */
	// // private Matrix d2L_dbdb = null;
	// private Matrix d2L_dbdb_BHHH = null;
	// private Matrix d2L_dbdb_add = null;
	//
	// /**
	// * Performs two tasks: First, it adds all terms for one agent n to the
	// * right-hand side sum in Equation (24). Second, it adds all terms
	// specific
	// * to agent n to the result of Equation (25).
	// *
	// * @param plans
	// * the choice set of agent n
	// * @param dL_dPn
	// * a vector that contains the derivatives of the sensor-data's
	// * log-likelihood with respect to all plan choice probabilities
	// * of agent n (elsewhere called the Lambda coefficients of those
	// * plans)
	// * @param dPn_db
	// * a matrix that contains in every row i the sensitivities of of
	// * plan i's choice probability with respect to the parameter
	// * vector b
	// * @param d2Pn_dbdb
	// * a list of matrices where the ith matrix contains the Hessian
	// * of choice probability i with respect to the parameter vector b
	// */
	// public void update_d2L_dbdb(final List<? extends Plan<L>> plans,
	// final Vector dL_dPn, final Matrix dPn_db,
	// final List<? extends Matrix> d2Pn_dbdb) {
	//
	// // if (this.d2L_dbdb == null) {
	// // this.d2L_dbdb = new Matrix(dPn_db.columnSize(), dPn_db.columnSize());
	// // }
	// if (this.d2L_dbdb_BHHH == null) {
	// this.d2L_dbdb_BHHH = new Matrix(dPn_db.columnSize(), dPn_db
	// .columnSize());
	// }
	// if (this.d2L_dbdb_add == null) {
	// this.d2L_dbdb_add = new Matrix(dPn_db.columnSize(), dPn_db
	// .columnSize());
	// }
	//
	// // update of right-hand side of Equation (24)
	// // omission leads to Gauss-Newton like approximation of the Hessian
	// if (d2Pn_dbdb != null) {
	// for (int i = 0; i < plans.size(); i++) {
	// this.d2L_dbdb_add.add(d2Pn_dbdb.get(i), dL_dPn.get(i));
	// }
	// }
	// // update of Equation (26)
	// for (int i = 0; i < plans.size(); i++) {
	// final Plan<L> plan = plans.get(i);
	// for (PlanStep<L> step : plan) {
	// final List<SingleLinkMeasurement<L>> measList = this.link2meas
	// .get(step.getLink());
	// if (measList != null) {
	// for (SingleLinkMeasurement<L> meas : measList) {
	// meas.register_dPni_db(dPn_db.getRow(i));
	// }
	// }
	// }
	// }
	// }
	//
	// /**
	// * Adds the left-hand side of Equation (24) to its right-hand side, which
	// is
	// * assumed to have been built through previous calls to
	// * update_dL_dParam(..). Uses results of Equation (25), which have been
	// * computed through the same previous function calls to
	// update_dL_dParam(..)
	// */
	// public void complete_d2L_dbdb() {
	// if (this.d2L_dbdb_BHHH == null) {
	// return;
	// }
	// for (SingleLinkMeasurement<L> meas : this.allSingleLinkMeas) {
	// final double var = meas.getMeasVariance();
	// final Vector dqak_db = meas.get_dqak_db();
	// if (dqak_db != null) {
	// for (int r = 0; r < this.d2L_dbdb_BHHH.rowSize(); r++) {
	// for (int s = 0; s < r; s++) {
	// final double rsAddend = (-1.0 / var) * dqak_db.get(r)
	// * dqak_db.get(s);
	// this.d2L_dbdb_BHHH.getRow(r).add(s, rsAddend);
	// this.d2L_dbdb_BHHH.getRow(s).add(r, rsAddend);
	// }
	// this.d2L_dbdb_BHHH.getRow(r).add(r,
	// (-1.0 / var) * dqak_db.get(r) * dqak_db.get(r));
	// }
	// }
	// }
	// }
	//
	// public Matrix get_d2L_dbdb_GS() {
	// return this.d2L_dbdb_BHHH;
	// }
	//
	// public Matrix get_d2L_dbdb_ADD() {
	// return this.d2L_dbdb_add;
	// }
	//
	// public void clear_d2L_dbdb() {
	// this.d2L_dbdb_BHHH = null;
	// this.d2L_dbdb_add = null;
	// for (SingleLinkMeasurement<L> meas : this.allSingleLinkMeas) {
	// meas.clear_dqak_db();
	// }
	// }
}

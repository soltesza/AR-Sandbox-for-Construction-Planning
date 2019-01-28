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
package floetteroed.cadyts.measurements;

import java.io.Serializable;
import java.util.Set;
import java.util.logging.Logger;

import floetteroed.cadyts.calibrators.Calibrator;
import floetteroed.cadyts.calibrators.TimedElement;
import floetteroed.cadyts.demand.Demand;
import floetteroed.cadyts.demand.Plan;
import floetteroed.cadyts.demand.PlanStep;
import floetteroed.cadyts.supply.LinkLoading;
import floetteroed.cadyts.supply.SimResults;
import floetteroed.utilities.math.SignalSmoother;

/**
 * 
 * @author Gunnar Flötteröd
 * 
 * @param L
 *            the link type
 * 
 */
public class SingleLinkMeasurement<L> extends TimedElement implements
		Serializable {

	// -------------------- TYPE DEFINITIONS --------------------

	public static enum TYPE {
		FLOW_VEH_H, COUNT_VEH
	};

	// -------------------- CONSTANTS --------------------

	private static final long serialVersionUID = 1L;

	// -------------------- CONSTANT MEMBERS --------------------

	private final TYPE type;

	private final double measValue;

	private final double measVariance;

	private final L link;

	// -------------------- MEMBERS --------------------

	private LinkLoading<L> loading;

	private SignalSmoother avgLinkFeature;

	private double lastLL = 0;

	private double lastLLPredErr = 0;

	private double lastLinkFeaturePredErr = 0;

	// -------------------- CONSTRUCTION --------------------

	public SingleLinkMeasurement(final L link, final double measValue,
			final double measVariance, final int startTime_s,
			final int endTime_s, final TYPE type) {

		super(startTime_s, endTime_s);

		// CHECK

		if (link == null) {
			throw new IllegalArgumentException("link is null");
		}
		if (Double.isNaN(measValue) || Double.isInfinite(measValue)) {
			throw new IllegalArgumentException("infeasible measurement value: "
					+ measValue);
		}
		if (measVariance <= 0 || Double.isNaN(measVariance)
				|| Double.isInfinite(measVariance)) {
			throw new IllegalArgumentException("infeasible variance value: "
					+ measVariance);
		}
		if (type == null) {
			throw new IllegalArgumentException("type must not be null");
		}

		// CONTINUE

		this.link = link;
		this.measValue = measValue;
		this.measVariance = measVariance;
		this.type = type;
		this.loading = null; // only until initialization
		this.avgLinkFeature = null; // only until initialization
	}

	public void init(final Calibrator<L> calibrator) {
		if (calibrator == null) {
			throw new IllegalArgumentException("calibrator must not be null");
		} else if (this.loading != null) {
			Logger.getLogger(this.getClass().getName()).warning(
					"init(..) has already been called; this call is ignored");
		} else {
			this.loading = calibrator.newLinkLoading(this.getLink(),
					this.getStartTime_s(), this.getEndTime_s(), this.getType());
			this.avgLinkFeature = new SignalSmoother(
					1.0 - calibrator.getRegressionInertia());
		}
	}

	// -------------------- SIMPLE FUNCTIONALITY --------------------

	public L getLink() {
		return this.link;
	}

	public double getMeasValue() {
		return this.measValue;
	}

	public double getMeasVariance() {
		return this.measVariance;
	}

	public double getMeasStddev() {
		return Math.sqrt(this.getMeasVariance());
	}

	public TYPE getType() {
		return this.type;
	}

	public Set<L> getRelevantLinks() {
		return this.loading.getRelevantLinks();
	}

	public boolean isPlanListening() {
		return this.loading.isPlanListening();
	}

	public void notifyPlanChoice(final Plan<L> plan) {
		this.loading.notifyPlanChoice(plan);
	}

	public void freeze() {
		this.loading.freeze();
		this.avgLinkFeature.freeze();
	}

	// -------------------- IMPLEMENTATION --------------------

	protected double ll(final double simValue) {
		final double e = simValue - this.getMeasValue();
		return (-1.0) * e * e / 2.0 / this.getMeasVariance();
	}

	protected double dll_dAvgLinkFeature() {
		return (this.getMeasValue() - this.avgLinkFeature.getSmoothedValue())
				/ this.getMeasVariance();
	}

	public void update(final Demand<L> demand, final SimResults<L> simResults) {

		final double linkFeature = simResults.getSimValue(this.getLink(),
				this.getStartTime_s(), this.getEndTime_s(), this.getType());
		final double predLinkFeature = this.loading.predictLinkFeature(demand);

		this.lastLL = this.ll(linkFeature);
		this.lastLLPredErr = this.lastLL - this.ll(predLinkFeature);
		this.lastLinkFeaturePredErr = linkFeature - predLinkFeature;

		this.avgLinkFeature.addValue(linkFeature);
		this.loading.update(demand, linkFeature);
	}

	public double getLambdaCoefficient(final L link) {
		double result = this.dll_dAvgLinkFeature()
				* this.loading.get_dLinkFeature_dDemand(link);
		return result;
	}

	public double getLambda(final PlanStep<L> step) {
		if (step.getEntryTime_s() < this.getStartTime_s()
				|| step.getEntryTime_s() >= this.getEndTime_s()) {
			return 0;
		} else {
			return this.getLambdaCoefficient(step.getLink());
		}
	}

	public double getLastLL() {
		return this.lastLL;
	}

	public double getLastLLPredErr() {
		return this.lastLLPredErr;
	}

	public double getLastLinkFeaturePredErr() {
		return this.lastLinkFeaturePredErr;
	}

	@Override
	public String toString() {
		final StringBuffer result = new StringBuffer();
		result.append(this.getClass().getSimpleName());
		result.append("(");
		result.append("type = ");
		result.append(this.getType());
		result.append(", link = ");
		result.append(this.getLink());
		result.append(", start_s = ");
		result.append(this.getStartTime_s());
		result.append(", dur_s = ");
		result.append(this.getDuration_s());
		result.append(", value = ");
		result.append(this.getMeasValue());
		result.append(", stddev = ");
		result.append(this.getMeasStddev());
		result.append(")");
		return result.toString();
	}

	// -------------------- PLAIN LL EVALUATION --------------------

	// TODO NEW
	public double logLikelihood(final Demand<L> demand) {
		return this.ll(this.loading.predictLinkFeature(demand));
	}

	// TODO NEW
	public double d_logLikelihood_d_linkDemand(final L link, final int time_s,
			final Demand<L> demand) {
		if (time_s < this.getStartTime_s() || time_s >= this.getEndTime_s()) {
			return 0;
		} else {
			return (this.getMeasValue() - this.loading
					.predictLinkFeature(demand))
					/ this.getMeasVariance()
					* this.loading.get_dLinkFeature_dDemand(link);
		}
	}

	// ---------- TODO NEW FOR LEVENBERG-MARCQUARDT ----------

	public double residual(final Demand<L> demand) {
		return (this.measValue - this.loading.predictLinkFeature(demand))
				/ Math.sqrt(this.measVariance);
	}

	// TODO this only works for single-link demand!
	public double dResidual_dLinkDemand() {
		return (-1.0) * this.loading.get_dLinkFeature_dDemand(this.link)
				/ Math.sqrt(this.measVariance);
	}

	// --------------------------------------------------------
	// -------------------- VERY OLD STUFF --------------------
	// --------------------------------------------------------

	// /*
	// * This is new code for the estimation of demand model parameters. It
	// * implements the incremental computation of Equation (25) for all choice
	// * model parameters. in the article [[TODO CITE]]. Variables are like in
	// * that article, only that "\Pi" is written as "P" and "\beta" as "b".
	// * [[TODO Move this into a subclass?]]
	// */
	//
	// /**
	// * A vector that contains the incrementally calculated result of Equation
	// * (25) for all choice model parameters.
	// */
	// private Vector dqak_db = null;
	//
	// /**
	// * Adds the sensitivity terms for plan i of agent n to the result of
	// * Equation (25); assumes that the plan actually crosses this link!
	// */
	// public void register_dPni_db(final Vector dPni_db) {
	// if (this.dqak_db == null) {
	// this.dqak_db = new Vector(dPni_db.size());
	// }
	// final double dq_dPni = this.loading.get_dLinkFeature_dDemand(this
	// .getLink());
	// this.dqak_db.add(dPni_db, dq_dPni);
	// }
	//
	// public Vector get_dqak_db() {
	// return this.dqak_db;
	// }
	//
	// public void clear_dqak_db() {
	// this.dqak_db = null;
	// }
}

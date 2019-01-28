/*
 * Opdyts - Optimization of dynamic traffic simulations
 *
 * Copyright 2015, 2016 Gunnar Flötteröd
 * 
 *
 * This file is part of Opdyts.
 *
 * Opdyts is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Opdyts is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Opdyts.  If not, see <http://www.gnu.org/licenses/>.
 *
 * contact: gunnar.floetteroed@abe.kth.se
 *
 */
package floetteroed.opdyts.trajectorysampling;

import static java.util.Collections.unmodifiableMap;

import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.Random;
import java.util.Set;
import java.util.logging.Logger;

import floetteroed.opdyts.DecisionVariable;
import floetteroed.opdyts.ObjectiveFunction;
import floetteroed.opdyts.SimulatorState;
import floetteroed.opdyts.convergencecriteria.ConvergenceCriterion;
import floetteroed.opdyts.convergencecriteria.ConvergenceCriterionResult;
import floetteroed.opdyts.logging.ConvergedObjectiveFunctionValue;
import floetteroed.opdyts.logging.EquilibriumGap;
import floetteroed.opdyts.logging.EquilibriumGapWeight;
import floetteroed.opdyts.logging.FreeMemory;
import floetteroed.opdyts.logging.LastDecisionVariable;
import floetteroed.opdyts.logging.LastEquilibriumGap;
import floetteroed.opdyts.logging.LastObjectiveFunctionValue;
import floetteroed.opdyts.logging.MaxMemory;
import floetteroed.opdyts.logging.SurrogateObjectiveFunctionValue;
import floetteroed.opdyts.logging.TotalMemory;
import floetteroed.opdyts.logging.UniformityGap;
import floetteroed.opdyts.logging.UniformityGapWeight;
import floetteroed.utilities.statisticslogging.Statistic;
import floetteroed.utilities.statisticslogging.StatisticsMultiWriter;

/**
 * 
 * @author Gunnar Flötteröd
 *
 */
public class MultiTrajectorySampler<U extends DecisionVariable> implements TrajectorySampler<U> {

	// -------------------- MEMBERS --------------------

	// set during construction

	private final ObjectiveFunction objectiveFunction;

	private final ConvergenceCriterion convergenceCriterion;

	private final Random rnd;

	private final double equilibriumWeight;

	private final double uniformityWeight;

	// minimum value is one per decision variable
	private final Map<U, Integer> decisionVariable2remainingWarmupIterations = new LinkedHashMap<>();

	// if set to false, only the last warm-up iteration is used
	private final boolean useAllWarmupIterations;

	// further program control parameters

	private final StatisticsMultiWriter<SamplingStage<U>> statisticsWriter;

	// runtime variables

	private int totalTransitionCnt = 0;

	private TransitionSequenceSet<U> allTransitionSequences;

	// the common initial state of all simulation trajectories
	private SimulatorState initialState = null;

	// the previously visited state
	private SimulatorState fromState = null;

	private U currentDecisionVariable = null;

	private Map<U, ConvergenceCriterionResult> decisionVariable2convergenceResult = new LinkedHashMap<>();

	private final List<SamplingStage<U>> samplingStages = new ArrayList<>();

	// -------------------- CONSTRUCTION --------------------

	public MultiTrajectorySampler(final Set<? extends U> decisionVariables, final ObjectiveFunction objectiveFunction,
			final ConvergenceCriterion convergenceCriterion, final Random rnd, final double equilibriumWeight,
			final double uniformityWeight, final boolean appendToLogFile, final int maxTotalMemory,
			final int maxMemoryPerTrajectory, final boolean maintainAllTrajectories, final int warmupIterations,
			final boolean useAllWarmupIterations) {
		this.useAllWarmupIterations = useAllWarmupIterations;
		for (U decisionVariable : decisionVariables) {
			this.decisionVariable2remainingWarmupIterations.put(decisionVariable, warmupIterations);
		}
		this.objectiveFunction = objectiveFunction;
		this.convergenceCriterion = convergenceCriterion;
		this.rnd = rnd;
		this.equilibriumWeight = equilibriumWeight;
		this.uniformityWeight = uniformityWeight;
		this.statisticsWriter = new StatisticsMultiWriter<>(appendToLogFile);
		this.allTransitionSequences = new TransitionSequenceSet<U>(maxTotalMemory, maxMemoryPerTrajectory,
				maintainAllTrajectories);
	}

	// -------------------- SETTERS AND GETTERS --------------------

	@Override
	public void addStatistic(final String logFileName, final Statistic<SamplingStage<U>> statistic) {
		this.statisticsWriter.addStatistic(logFileName, statistic);
	}

	@Override
	public void setStandardLogFileName(final String logFileName) {
		this.addStatistic(logFileName, new SurrogateObjectiveFunctionValue<U>());
		this.addStatistic(logFileName, new LastObjectiveFunctionValue<U>());
		this.addStatistic(logFileName, new ConvergedObjectiveFunctionValue<U>());
		this.addStatistic(logFileName, new EquilibriumGapWeight<U>());
		this.addStatistic(logFileName, new EquilibriumGap<U>());
		this.addStatistic(logFileName, new LastEquilibriumGap<U>());
		this.addStatistic(logFileName, new UniformityGapWeight<U>());
		this.addStatistic(logFileName, new UniformityGap<U>());
		this.addStatistic(logFileName, new TotalMemory<U>());
		this.addStatistic(logFileName, new FreeMemory<U>());
		this.addStatistic(logFileName, new MaxMemory<U>());
		this.addStatistic(logFileName, new LastDecisionVariable<U>());
	}

	@Override
	public U getCurrentDecisionVariable() {
		return this.currentDecisionVariable;
	}

	public int getTotalTransitionCnt() {
		return this.totalTransitionCnt;
	}

	@Override
	public Map<U, ConvergenceCriterionResult> getDecisionVariable2convergenceResultView() {
		return unmodifiableMap(this.decisionVariable2convergenceResult);
	}

	@Override
	public boolean foundSolution() {
		return (this.decisionVariable2convergenceResult.size() > 0);
	}

	public List<SamplingStage<U>> getSamplingStages() {
		return this.samplingStages;
	}

	public List<Transition<U>> getTransitions() {
		return this.allTransitionSequences.getAllTransitionsInInsertionOrder();
	}

	@Override
	public ObjectiveFunction getObjectiveFunction() {
		return this.objectiveFunction;
	}

	public List<Transition<U>> getTransitions(final U decisionVariable) {
		return this.allTransitionSequences.getTransitions(decisionVariable);
	}

	public int additionCnt(final U decisionVariable) {
		return this.allTransitionSequences.additionCnt(decisionVariable);
	}

	// -------------------- IMPLEMENTATION --------------------

	public void afterIteration(final SimulatorState newState) {

		/*
		 * When this function is called for the first time, all of the following
		 * variables are null: currentDecisionVariable, initialState, fromState.
		 * 
		 * Implications:
		 * 
		 * The simulation needs to ensure that a sensible initial decision
		 * variable is used. This should be the decision variable of which
		 * variations are to be tried out (i.e. the "mean" decision variable).
		 * 
		 * Since currentDecisionVariable, initialState, fromState are
		 * initialized with null values only at construction time, instances of
		 * this class cannot be recycled.
		 */

		this.totalTransitionCnt++;
		Logger.getLogger(this.getClass().getName()).info("Trajectory sampling iteration " + this.totalTransitionCnt);

		/*
		 * If the currentDecisionVariable is null then one has just observed the
		 * first simulator transition after initialization; not much can be
		 * learned from that. (The currentDecisionVariable being null implies
		 * both the fromState and the initialState being null.)
		 * 
		 * If the currentDecisionVariable is not null, a full transition has
		 * been observed that can now be memorized for (later) processing. If
		 * this is done depends on if it is a warm-up iteration and what the
		 * configuration for using such iterations is.
		 */

		if ((this.currentDecisionVariable != null) && (this.useAllWarmupIterations
				|| (this.decisionVariable2remainingWarmupIterations.get(this.currentDecisionVariable) == null))) {

			this.allTransitionSequences.addTransition(this.fromState, this.currentDecisionVariable, newState,
					this.objectiveFunction.value(newState));

		}

		/*
		 * Prepare the next iteration.
		 */

		if (this.decisionVariable2remainingWarmupIterations.size() > 0) {

			// Still in the warm-up phase.

			if (this.currentDecisionVariable == null) {
				// very first iteration
				this.initialState = newState;
				this.fromState = newState;
			} else if (!this.decisionVariable2remainingWarmupIterations.containsKey(this.currentDecisionVariable)) {
				// switch to new warm-up trajectory
				this.fromState = this.initialState;
				this.fromState.implementInSimulation();
			} else {
				// continue a warm-up trajectory
				this.fromState = newState;
			}

			this.currentDecisionVariable = this.decisionVariable2remainingWarmupIterations.keySet().iterator().next(); // relies
			this.currentDecisionVariable.implementInSimulation();

			{
				final int remainingWarmupIterations = this.decisionVariable2remainingWarmupIterations
						.get(this.currentDecisionVariable);
				if (remainingWarmupIterations == 1) {
					this.decisionVariable2remainingWarmupIterations.remove(this.currentDecisionVariable);
				} else {
					this.decisionVariable2remainingWarmupIterations.put(this.currentDecisionVariable,
							remainingWarmupIterations - 1);
				}
			}

			this.statisticsWriter.writeToFile(null, EquilibriumGapWeight.LABEL, Double.toString(this.equilibriumWeight),
					UniformityGapWeight.LABEL, Double.toString(this.uniformityWeight));

		} else {

			// Warm-up phase is over.

			// Check for convergence.
			final ConvergenceCriterionResult convergenceResult = this.convergenceCriterion.evaluate(
					this.allTransitionSequences.getTransitions(this.currentDecisionVariable),
					this.allTransitionSequences.additionCnt(this.currentDecisionVariable));
			if (convergenceResult.converged) {
				this.decisionVariable2convergenceResult.put(this.currentDecisionVariable, convergenceResult);
			}

			// Process the most recent sampling stage.
			final TransitionSequencesAnalyzer<U> samplingStageEvaluator = new TransitionSequencesAnalyzer<U>(
					this.allTransitionSequences.getAllTransitionsInInsertionOrder(), this.equilibriumWeight,
					this.uniformityWeight);
			final SamplingStage<U> samplingStage = samplingStageEvaluator.newOptimalSamplingStage(
					this.allTransitionSequences.getTransitions(this.currentDecisionVariable).getLast(),
					convergenceResult.finalObjectiveFunctionValue, (this.samplingStages.size() == 0) ? null
							: this.samplingStages.get(samplingStages.size() - 1).transition2lastSolutionView());
			this.samplingStages.add(samplingStage);
			this.statisticsWriter.writeToFile(samplingStage);

			// Decide what to do next.
			this.currentDecisionVariable = samplingStage.drawDecisionVariable(this.rnd);
			this.fromState = this.allTransitionSequences.getLastState(this.currentDecisionVariable);
			this.fromState.implementInSimulation();
			this.currentDecisionVariable.implementInSimulation();

		}
	}
}

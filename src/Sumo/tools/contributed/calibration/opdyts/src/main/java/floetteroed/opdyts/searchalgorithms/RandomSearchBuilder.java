package floetteroed.opdyts.searchalgorithms;

import floetteroed.opdyts.DecisionVariable;
import floetteroed.opdyts.DecisionVariableRandomizer;
import floetteroed.opdyts.ObjectiveFunction;
import floetteroed.opdyts.convergencecriteria.ConvergenceCriterion;

/**
 * 
 * @author Kai Nagel
 *
 * @param <U>
 *            the decision variable type
 */
public class RandomSearchBuilder<U extends DecisionVariable> {

	private Simulator<U> simulator = null;
	private DecisionVariableRandomizer<U> randomizer = null;
	private U initialDecisionVariable = null;
	private ConvergenceCriterion convergenceCriterion = null;
	private int maxIterations = 10;
	private int maxTransitions = Integer.MAX_VALUE;
	private int populationSize = 10;
	// private Random rnd = new Random(4711);
	// private boolean interpolate = true;
	private ObjectiveFunction objectiveFunction = null;
	// private boolean includeCurrentBest = false;
	// private int warmupIterations = 1;
	// private boolean useAllWarmupIterations = false;

	private static void assertTrue(boolean condition) {
		if (!condition) {
			throw new RuntimeException("something is wrong; follow stack trace");
		}
	}

	private static void assertNotNull(Object object) {
		assertTrue(object != null);
	}

	/**
	 * See {@link Simulator}.
	 * 
	 * For default value see code of {@link RandomSearch.Builder}.
	 */
	public final RandomSearchBuilder<U> setSimulator(Simulator<U> simulator) {
		this.simulator = simulator;
		return this;
	}

	/**
	 * Very problem-specific: given a decision variable, create trial variations
	 * thereof. These variations should be large enough to yield a measurable
	 * change in objective function value but they should still be relatively
	 * small (in the sense of a local search).
	 * 
	 * From the experiments performed so far, it appears as if the number of
	 * trial decision variables should be as large as memory allows.
	 *
	 * For default value see code of {@link RandomSearch.Builder}.
	 */
	public final RandomSearchBuilder<U> setRandomizer(DecisionVariableRandomizer<U> randomizer) {
		this.randomizer = randomizer;
		return this;
	}

	/**
	 * The starting point of the search. The initial simulation configuration
	 * should be such the simulation is converged given this decision variable.
	 * 
	 * For default value see code of {@link RandomSearch.Builder}.
	 */
	public final RandomSearchBuilder<U> setInitialDecisionVariable(U initialDecisionVariable) {
		this.initialDecisionVariable = initialDecisionVariable;
		return this;
	}

	/**
	 * Defines the convergence criterion.
	 * 
	 * This requires to set (i) the number of iterations until the simulation
	 * has converged and (ii) the number of iterations over which to average to
	 * get rid of the simulation noise.
	 * 
	 * (i) The number of iterations until the simulation has converged is
	 * relative to the amount of variability in the decision variable
	 * randomization. Let X be any decision variable and Y be a random variation
	 * thereof. Let the simulation start with a converged plans file obtained
	 * with decision variable X. The number of iterations must then be large
	 * enough to reach a new converged state for any decision variable Y.
	 * 
	 * (ii) The number of iterations over which to average should be large
	 * enough to make the remaining simulation noise small compared to the
	 * expected difference between the objective function values of any decision
	 * variable and its random variation.
	 *
	 * For default value see code of {@link RandomSearch.Builder}.
	 */
	public final RandomSearchBuilder<U> setConvergenceCriterion(ConvergenceCriterion convergenceCriterion) {
		this.convergenceCriterion = convergenceCriterion;
		return this;
	}

	/**
	 * Maximum number of random search iterations.
	 * 
	 * For default value see code of {@link RandomSearch.Builder}.
	 */
	public final RandomSearchBuilder<U> setMaxIterations(int maxIterations) {
		this.maxIterations = maxIterations;
		return this;
	}

	/**
	 * Maximum total number of evaluated simulator transitions.
	 * 
	 * For default value see code of {@link RandomSearch.Builder}.
	 */
	public final RandomSearchBuilder<U> setMaxTransitions(int maxTransitions) {
		this.maxTransitions = maxTransitions;
		return this;
	}

	/**
	 * How many candidate decision variables are to be created. Based on
	 * empirical experience, the more the better. Available memory defines an
	 * upper limit.
	 * 
	 * For default value see code of {@link RandomSearch.Builder}.
	 */
	public final RandomSearchBuilder<U> setPopulationSize(int populationSize) {
		this.populationSize = populationSize;
		return this;
	}

	// /**
	// * For default value see code of {@link RandomSearch.Builder}.
	// */
	// public final RandomSearchBuilder<U> setRnd(Random rnd) {
	// this.rnd = rnd;
	// return this;
	// }

	// /**
	// * For all practical purposes, keep this "true". "false" is only for
	// * debugging.
	// *
	// * For default value see code of {@link RandomSearch.Builder}.
	// */
	// public final RandomSearchBuilder<U> setInterpolate(boolean interpolate) {
	// this.interpolate = interpolate;
	// return this;
	// }

	/**
	 * The objective function: a quantitative measure of what one wants to
	 * achieve. To be minimized.
	 * 
	 * For default value see code of {@link RandomSearch.Builder}.
	 */
	public final RandomSearchBuilder<U> setObjectiveFunction(ObjectiveFunction objectiveFunction) {
		this.objectiveFunction = objectiveFunction;
		return this;
	}

	// /**
	// * If the currently best decision variable is to be included in the set of
	// * new candidate decision variables. More an experimental feature, better
	// * keep it "false".
	// *
	// * For default value see code of {@link RandomSearch.Builder}.
	// */
	// public final RandomSearchBuilder<U> setIncludeCurrentBest(boolean
	// includeCurrentBest) {
	// this.includeCurrentBest = includeCurrentBest;
	// return this;
	// }

	// public final RandomSearchBuilder<U> setWarmupIterations(int
	// warmupIterations) {
	// this.warmupIterations = warmupIterations;
	// return this;
	// }

	// public final RandomSearchBuilder<U> setUseAllWarmupIterations(boolean
	// useAllWarmupIterations) {
	// this.useAllWarmupIterations = useAllWarmupIterations;
	// return this;
	// }

	public final RandomSearch<U> build() {
		assertNotNull(simulator);
		assertNotNull(randomizer);
		assertNotNull(initialDecisionVariable);
		assertNotNull(convergenceCriterion);
		assertTrue(maxIterations > 0);
		assertTrue(maxTransitions > 0);
		assertTrue(populationSize > 0);
		assertNotNull(objectiveFunction);
		return new RandomSearch<>(simulator, randomizer, initialDecisionVariable, convergenceCriterion, maxIterations,
				maxTransitions, populationSize, objectiveFunction);
	}
}

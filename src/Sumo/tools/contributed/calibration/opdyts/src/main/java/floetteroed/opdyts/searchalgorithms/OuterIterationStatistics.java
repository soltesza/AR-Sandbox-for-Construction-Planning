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
package floetteroed.opdyts.searchalgorithms;

import java.text.SimpleDateFormat;

import floetteroed.utilities.statisticslogging.Statistic;
import floetteroed.utilities.statisticslogging.StatisticsWriter;

/**
 * 
 * @author Gunnar Flötteröd
 *
 */
public class OuterIterationStatistics {

	// -------------------- STATIC FUNCTIONALITY --------------------

	public static final String INITIAL_TIMESTAMP = "Initial Timestamp";

	public static final String ITERATION_NUMBER = "Iteration Number";

	public static final String INITIAL_TRANSITION_NUMBER = "Initial Transition Number";

	public static final String INITIAL_EQUILIBRIUM_GAP_WEIGHT = "Initial Equilibrium Gap Weight";

	public static final String INITIAL_UNIFORMITY_GAP_WEIGHT = "Initial Uniformity Gap Weight";

	public static final String FINAL_OBJECTIVE_FUNCTION_VALUE = "Final Objective Function Value";

	public static final String ADDITIONAL_TRANSITION_NUMBER = "Additional Transition Number";

	public static final String FINAL_TIMESTAMP = "Final Timestamp";

	private static String emptyStrForNull(final Object data) {
		return (data == null ? "" : data.toString());
	}

	static void initializeWriter(
			final StatisticsWriter<OuterIterationStatistics> writer) {

		writer.addSearchStatistic(new Statistic<OuterIterationStatistics>() {
			@Override
			public String label() {
				return INITIAL_TIMESTAMP;
			}

			@Override
			public String value(final OuterIterationStatistics data) {
				if (data.initialTime_ms != null) {
					return (new SimpleDateFormat("yyyy-MM-dd HH:mm:ss"))
							.format(data.initialTime_ms);
				} else {
					return "";
				}
			}

		});

		writer.addSearchStatistic(new Statistic<OuterIterationStatistics>() {
			@Override
			public String label() {
				return ITERATION_NUMBER;
			}

			@Override
			public String value(final OuterIterationStatistics data) {
				return emptyStrForNull(data.iterationNumber);
			}

		});

		writer.addSearchStatistic(new Statistic<OuterIterationStatistics>() {
			@Override
			public String label() {
				return INITIAL_TRANSITION_NUMBER;
			}

			@Override
			public String value(final OuterIterationStatistics data) {
				return emptyStrForNull(data.initialTransitionNumber);
			}

		});

		writer.addSearchStatistic(new Statistic<OuterIterationStatistics>() {
			@Override
			public String label() {
				return INITIAL_EQUILIBRIUM_GAP_WEIGHT;
			}

			@Override
			public String value(final OuterIterationStatistics data) {
				return emptyStrForNull(data.initialEquilibriumGapWeight);
			}

		});

		writer.addSearchStatistic(new Statistic<OuterIterationStatistics>() {
			@Override
			public String label() {
				return INITIAL_UNIFORMITY_GAP_WEIGHT;
			}

			@Override
			public String value(final OuterIterationStatistics data) {
				return emptyStrForNull(data.initialUniformityGapWeight);
			}

		});

		writer.addSearchStatistic(new Statistic<OuterIterationStatistics>() {
			@Override
			public String label() {
				return FINAL_OBJECTIVE_FUNCTION_VALUE;
			}

			@Override
			public String value(final OuterIterationStatistics data) {
				return emptyStrForNull(data.finalObjectiveFunctionValue);
			}

		});

		writer.addSearchStatistic(new Statistic<OuterIterationStatistics>() {
			@Override
			public String label() {
				return ADDITIONAL_TRANSITION_NUMBER;
			}

			@Override
			public String value(final OuterIterationStatistics data) {
				return emptyStrForNull(data.additionalTransitionNumber);
			}

		});

		writer.addSearchStatistic(new Statistic<OuterIterationStatistics>() {
			@Override
			public String label() {
				return FINAL_TIMESTAMP;
			}

			@Override
			public String value(final OuterIterationStatistics data) {
				if (data.finalTime_ms != null) {
					return (new SimpleDateFormat("yyyy-MM-dd HH:mm:ss"))
							.format(data.finalTime_ms);
				} else {
					return "";
				}
			}

		});
	}

	// -------------------- MEMBERS --------------------

	private final Long initialTime_ms;

	private final Integer iterationNumber;

	private final Integer initialTransitionNumber;

	private final Double initialEquilibriumGapWeight;

	private final Double initialUniformityGapWeight;

	private boolean finalized = false;

	private Double finalObjectiveFunctionValue = null;

	private Integer additionalTransitionNumber = null;

	private Long finalTime_ms = null;

	// -------------------- CONSTRUCTION --------------------

	OuterIterationStatistics(final Long initialTime_ms,
			final Integer iterationNumber,
			final Integer initialTransitinNumber,
			final Double initialEquilibriumGapWeight,
			final Double initialUniformityGapWeight) {
		this.initialTime_ms = initialTime_ms;
		this.iterationNumber = iterationNumber;
		this.initialTransitionNumber = initialTransitinNumber;
		this.initialEquilibriumGapWeight = initialEquilibriumGapWeight;
		this.initialUniformityGapWeight = initialUniformityGapWeight;
		this.finalized = false;
	}

	void finalize(final Double finalObjectiveFunctionValue,
			final Integer additionalTransitionNumber, final Long finalTime_ms) {
		if (this.finalized) {
			throw new RuntimeException(this.getClass().getSimpleName()
					+ " is already finalized.");
		}
		this.finalized = true;
		this.finalObjectiveFunctionValue = finalObjectiveFunctionValue;
		this.finalTime_ms = finalTime_ms;
		this.additionalTransitionNumber = additionalTransitionNumber;
	}
}

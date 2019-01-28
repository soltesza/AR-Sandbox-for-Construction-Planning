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
package floetteroed.opdyts.logging;

import floetteroed.opdyts.DecisionVariable;
import floetteroed.opdyts.trajectorysampling.SamplingStage;
import floetteroed.utilities.statisticslogging.Statistic;

/**
 * 
 * @author Gunnar Flötteröd
 *
 */
public abstract class AbstractDecisionVariableAverage<U extends DecisionVariable>
		implements Statistic<SamplingStage<U>> {

	// -------------------- CONSTRUCTION --------------------

	public AbstractDecisionVariableAverage() {
	}

	// --------------- IMPLEMENTATION OF SearchStatistic ---------------

	@Override
	public String label() {
		return "Average " + this.realValueLabel();
	}

	@Override
	public String value(final SamplingStage<U> samplingStage) {
		double average = 0;
		for (U decisionVariable : samplingStage.getDecisionVariables()) {
			average += samplingStage.getAlphaSum(decisionVariable)
					* this.realValue(decisionVariable);
		}
		return Double.toString(average);
	}

	// -------------------- INTERFACE DEFINITION --------------------

	public abstract String realValueLabel();

	public abstract double realValue(DecisionVariable decisionVariable);
}

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
package floetteroed.opdyts.convergencecriteria;

/**
 * 
 * @author Gunnar Flötteröd
 *
 */
public class ConvergenceCriterionResult {

	public final boolean converged;
	
	public final Double finalObjectiveFunctionValue;

	public final Double finalObjectiveFunctionValueStddev;

	public final Double finalEquilbiriumGap;

	public final Double finalUniformityGap;

	public final Object lastDecisionVariable;

	public final Integer lastTransitionSequenceLength;

	public ConvergenceCriterionResult(final boolean converged, final Double finalObjectiveFunctionValue,
			final Double finalObjectiveFunctionValueStddev,
			final Double finalEquilibiriumGap, final Double finalUniformityGap,
			final Object lastDecisionVariable,
			final Integer lastTransitionSequenceLength) {
		this.converged = converged;
		this.finalObjectiveFunctionValue = finalObjectiveFunctionValue;
		this.finalObjectiveFunctionValueStddev = finalObjectiveFunctionValueStddev;
		this.finalEquilbiriumGap = finalEquilibiriumGap;
		this.finalUniformityGap = finalUniformityGap;
		this.lastDecisionVariable = lastDecisionVariable;
		this.lastTransitionSequenceLength = lastTransitionSequenceLength;
	}
}

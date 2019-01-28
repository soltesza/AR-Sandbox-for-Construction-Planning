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
package floetteroed.opdyts;

import java.util.Collection;

/**
 * Creates random decision variables that are used as "innovations" in an
 * iterative, stochastic optimization process.
 * 
 * @param U
 *            the decision variable type
 * 
 * @author Gunnar Flötteröd
 *
 */
public interface DecisionVariableRandomizer<U extends DecisionVariable> {

	/**
	 * The result should contain at least one random variation of
	 * decisionVariable.
	 * <p>
	 * It is, however, recommended to implement one of the following sampling
	 * strategies:
	 * <ul>
	 * <li>Return two random variations of decisionVariable that are symmetric (
	 * "positive and negative") to the extent possible.
	 * <li>Return an even larger number of decision variable variations (up to
	 * the total number of candidate decision variables specified in
	 * RandomSearch) by some experimental plan.
	 * </ul>
	 * <p>
	 */
	public Collection<U> newRandomVariations(final U decisionVariable);

}

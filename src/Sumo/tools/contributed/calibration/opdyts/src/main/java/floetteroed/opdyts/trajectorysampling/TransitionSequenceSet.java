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

import java.util.LinkedHashMap;
import java.util.LinkedHashSet;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.Set;

import floetteroed.opdyts.DecisionVariable;
import floetteroed.opdyts.SimulatorState;

/**
 * 
 * @author Gunnar Flötteröd
 *
 */
class TransitionSequenceSet<U extends DecisionVariable> {

	// -------------------- MEMBERS --------------------

	private final int maxTotalMemory;

	private final int maxMemoryPerTrajectory;

	private final boolean maintainAllTrajectories;

	private final Map<U, TransitionSequence<U>> decisionVariable2transitionSequence = new LinkedHashMap<>();

	private final LinkedList<Transition<U>> transitionsInInsertionOrder = new LinkedList<Transition<U>>();

	// -------------------- CONSTRUCTION --------------------

	TransitionSequenceSet(final int maxTotalMemory,
			final int maxMemoryPerTrajectory,
			final boolean maintainAllTrajectories) {
		this.maxTotalMemory = maxTotalMemory;
		this.maxMemoryPerTrajectory = maxMemoryPerTrajectory;
		this.maintainAllTrajectories = maintainAllTrajectories;
	}

	// -------------------- SETTERS --------------------

	void addTransition(final SimulatorState fromState,
			final U decisionVariable, final SimulatorState toState,
			final double objectiveFunctionValue) {

		/*
		 * Add the new Transition to the individual TransitionSequence and the
		 * chronological list of all transitions.
		 */

		TransitionSequence<U> transitionSequence = this.decisionVariable2transitionSequence
				.get(decisionVariable);
		if (transitionSequence == null) {
			transitionSequence = new TransitionSequence<U>(fromState,
					decisionVariable, toState, objectiveFunctionValue);
			this.decisionVariable2transitionSequence.put(decisionVariable,
					transitionSequence);
		} else {
			transitionSequence.addTransition(fromState, decisionVariable,
					toState, objectiveFunctionValue);
		}
		this.transitionsInInsertionOrder.add(transitionSequence
				.getLastTransition());

		/*
		 * Ensure that the individual TransitionSequence is not too long. If
		 * necessary, remove also the corresponding elements in the
		 * chronological list of all transitions.
		 */
		final List<Transition<U>> removed = transitionSequence
				.shrinkToMaximumLength(this.maxMemoryPerTrajectory);
		this.transitionsInInsertionOrder.removeAll(removed);
		
		/*
		 * If the chronological list of all transitions is too long, remove as
		 * many transitions as necessary and possible.
		 */

		final Set<Transition<U>> transitionsToMaintain = new LinkedHashSet<>();
		if (this.maintainAllTrajectories) {
			for (TransitionSequence<U> sequence : this.decisionVariable2transitionSequence
					.values()) {
				transitionsToMaintain.add(sequence.getLastTransition());
			}
		}

		int candidateRemovalIndex = 0;
		while (Math.max(this.maxTotalMemory, candidateRemovalIndex) < this.transitionsInInsertionOrder
				.size()) {
			final Transition<U> candidateRemovalTransition = this.transitionsInInsertionOrder
					.get(candidateRemovalIndex);
			if (!transitionsToMaintain.contains(candidateRemovalTransition)) {
				this.transitionsInInsertionOrder
						.remove(candidateRemovalTransition);
			} else {
				candidateRemovalIndex++;
			}
		}
	}

	// -------------------- GETTERS --------------------

	int size() {
		return this.transitionsInInsertionOrder.size();
	}

	int additionCnt(final U decisionVariable) {
		return this.decisionVariable2transitionSequence.get(decisionVariable)
				.additionCnt();
	}

	LinkedList<Transition<U>> getTransitions(final U decisionVariable) {
		return this.decisionVariable2transitionSequence.get(decisionVariable)
				.getTransitions();
	}

	LinkedList<Transition<U>> getAllTransitionsInInsertionOrder() {
		return this.transitionsInInsertionOrder;
	}

	SimulatorState getLastState(final U decisionVariable) {
		return this.decisionVariable2transitionSequence.get(decisionVariable)
				.getLastState();
	}
}

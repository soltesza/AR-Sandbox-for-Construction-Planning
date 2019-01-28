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
package floetteroed.cadyts.demand;

import floetteroed.cadyts.calibrators.Calibrator;

/**
 * 
 * @author Gunnar Flötteröd
 * 
 * @param <L>
 *            the link type
 */
public class PlanBuilder<L> {

	// -------------------- MEMBERS --------------------

	private final boolean countFirstLink;

	private final boolean countLastLink;

	private BasicPlan<L> result;

	// -------------------- CONSTRUCTION --------------------

	public PlanBuilder(final boolean countFirstLink, final boolean countLastLink) {
		this.countFirstLink = countFirstLink;
		this.countLastLink = countLastLink;
		reset();
	}
	
	public PlanBuilder() {
		this(Calibrator.DEFAULT_COUNT_FIRST_LINK, Calibrator.DEFAULT_COUNT_LAST_LINK);
	}

	// -------------------- PLAN BUILDING PROCEDURE --------------------

	public void reset() {
		this.result = new BasicPlan<L>();
	}

	public void reset(final BasicPlan<L> result) {
		this.result = result;
	}

	public void addEntry(L entryLink, int time_s) {
		if (this.countFirstLink) {
			this.result.addStep(new PlanStep<L>(entryLink, time_s));
		}
	}

	public void addTurn(L toLink, int time_s) {
		this.result.addStep(new PlanStep<L>(toLink, time_s));
	}

	public void addExit(int time_s) {
		if (!this.countLastLink && this.result.size() > 0) {
			this.result.removeLastStep();
		}
	}

	public Plan<L> getResult() {
		this.result.trim();
		return this.result;
	}
}

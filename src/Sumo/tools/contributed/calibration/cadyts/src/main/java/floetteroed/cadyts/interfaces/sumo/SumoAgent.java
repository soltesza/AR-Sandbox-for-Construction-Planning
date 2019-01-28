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
package floetteroed.cadyts.interfaces.sumo;

import java.util.List;
import java.util.logging.Logger;

import floetteroed.cadyts.calibrators.filebased.Agent;
import floetteroed.cadyts.demand.ODRelation;
import floetteroed.cadyts.demand.PlanChoiceDistribution;
import floetteroed.utilities.Tuple;
import floetteroed.utilities.math.Vector;

/**
 * 
 * @author Gunnar Flötteröd
 * 
 */
class SumoAgent extends Agent<SumoPlan, PlanChoiceDistribution<SumoPlan>> {

	// -------------------- MEMBERS --------------------

	private final int dpt_s;

	private final ODRelation<String> odRelation;

	private final boolean isClone;

	private final List<Tuple<String, String>> miscAttrs;

	// -------------------- CONSTRUCTION --------------------

	SumoAgent(final String id, final int dpt_s, final String fromTAZ,
			final String toTAZ, boolean isClone,
			final List<Tuple<String, String>> miscAttrs) {
		super(id, new PlanChoiceDistribution<SumoPlan>());
		this.dpt_s = dpt_s;
		if (fromTAZ != null && toTAZ != null) {
			this.odRelation = new ODRelation<String>(fromTAZ, toTAZ);
		} else {
			this.odRelation = null;
		}
		this.isClone = isClone;
		this.miscAttrs = miscAttrs;
	}

	SumoAgent(final String id, final int dpt_s, final String fromTAZ,
			final String toTAZ, final List<Tuple<String, String>> miscAttrs) {
		this(id, dpt_s, fromTAZ, toTAZ, false, miscAttrs);
	}

	SumoAgent(final SumoAgent parent, final String idPostfix) {
		this(parent.getId() + idPostfix, parent.getDptTime_s(), parent
				.getFromTAZ(), parent.getToTAZ(), true, parent.getMiscAttrs());
		final List<SumoPlan> plans = parent.getPlans();
		final Vector probs = parent.getPlanChoiceModel()
				.getChoiceProbabilities(plans);
		for (int i = 0; i < plans.size(); i++) {
			this.addPlan(plans.get(i), probs.get(i));
		}
	}

	// -------------------- CONTENT WRITE ACCESS --------------------

	void addPlan(final SumoPlan plan, final double choiceProb) {
		Logger.getLogger(this.getClass().getName()).fine(
				"agent " + this.getId() + " received route "
						+ plan.getRouteId());
		this.addPlan(plan);
		this.getPlanChoiceModel().addChoiceProbability(choiceProb);
	}

	// -------------------- CONTENT READ ACCESS --------------------

	int getDptTime_s() {
		return this.dpt_s;
	}

	ODRelation<String> getODRelation() {
		return this.odRelation;
	}

	String getFromTAZ() {
		return (this.getODRelation() != null ? this.getODRelation()
				.getFromTAZ() : null);
	}

	String getToTAZ() {
		return (this.getODRelation() != null ? this.getODRelation().getToTAZ()
				: null);
	}

	boolean isClone() {
		return this.isClone;
	}

	List<Tuple<String, String>> getMiscAttrs() {
		return this.miscAttrs;
	}
}

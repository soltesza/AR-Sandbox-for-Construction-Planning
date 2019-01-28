/*
 * BIOROUTE
 *
 * Copyright 2011-2016 Gunnar Flötteröd and Michel Bierlaire
 * 
 *
 * This file is part of BIOROUTE.
 *
 * BIOROUTE is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * BIOROUTE is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with BIOROUTE.  If not, see <http://www.gnu.org/licenses/>.
 *
 * contact: gunnar.floetteroed@abe.kth.se
 *
 */ 
package floetteroed.bioroute.pathgenerator.metropolishastings;

import static floetteroed.bioroute.BiorouteRunner.PATHGENERATOR_ELEMENT;
import static floetteroed.bioroute.pathgenerator.metropolishastings.MHPathGenerator.RELATIVECOSTSCALE_ELEMENT;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import floetteroed.utilities.config.Config;
import floetteroed.utilities.math.MathHelpers;
import floetteroed.utilities.math.metropolishastings.MHWeight;
import floetteroed.utilities.networks.basic.BasicLink;
import floetteroed.utilities.networks.basic.BasicNetwork;
import floetteroed.utilities.networks.shortestpaths.LinkCost;


/**
 * Computes link costs and path weights.
 * 
 * @author Gunnar Flötteröd
 * 
 */
class MHLinkAndPathCost implements LinkCost, MHWeight<MHPath> {

	// -------------------- CONSTANTS --------------------

	// XML CONFIGURATION

	static final String LINKCOST_ELEMENT = "linkcost";

	static final String ATTRIBUTE_ELEMENT = "attribute";

	static final String COEFFICIENT_ELEMENT = "coefficient";

	static final String LINKCOSTSCALE_ELEMENT = "linkcostscale";

	// TODO REMOVED
	// static final String NODELOOPSCALE_ELEMENT = "nodeloopscale";

	// DEFAULT VALUES

	// TODO REMOVED
	// static final double DEFAULT_NODELOOPSCALE = 0.0;

	// -------------------- MEMBERS --------------------

	// CONFIGURATION

	private List<String> attributes = null;

	private List<Double> coefficients = null;

	private Double linkCostScale = null;

	// TODO REMOVED
	// private Double nodeLoopScale = null;

	// RUNTIME

	private Map<BasicLink, Double> link2weight = null;

	// -------------------- CONSTRUCTION --------------------

	public MHLinkAndPathCost() {
		// no-argument constructor for reflective instantiation
	}

	// -------------------- INITIALIZATION --------------------

	void configure(final Config config) {

		this.attributes = config.getList(PATHGENERATOR_ELEMENT,
				LINKCOST_ELEMENT, ATTRIBUTE_ELEMENT);
		if (this.attributes == null || this.attributes.size() == 0) {
			throw new IllegalArgumentException("attributes are not specified");
		}

		final List<String> coeffStrs = config.getList(PATHGENERATOR_ELEMENT,
				LINKCOST_ELEMENT, COEFFICIENT_ELEMENT);
		if (coeffStrs == null || coeffStrs.size() != this.attributes.size()) {
			throw new IllegalArgumentException(
					"coefficients do not match attributes");
		}
		this.coefficients = new ArrayList<Double>(coeffStrs.size());
		for (String coeffStr : coeffStrs) {
			this.coefficients.add(Double.parseDouble(coeffStr));
		}

		this.linkCostScale = MathHelpers.parseDouble(config.get(
				PATHGENERATOR_ELEMENT, LINKCOSTSCALE_ELEMENT));
		if (this.linkCostScale == null
				&& config.get(PATHGENERATOR_ELEMENT, RELATIVECOSTSCALE_ELEMENT) == null) {
			throw new IllegalArgumentException(LINKCOSTSCALE_ELEMENT
					+ " is unspecified but " + RELATIVECOSTSCALE_ELEMENT
					+ " is unspecified as well");
		}

		// TODO REMOVED
		// this.nodeLoopScale = MathHelpers.parseDouble(config.get(
		// PATHGENERATOR_ELEMENT, NODELOOPSCALE_ELEMENT));
		// if (this.nodeLoopScale == null) {
		// this.nodeLoopScale = DEFAULT_NODELOOPSCALE;
		// }
	}

	// -------------------- SETTERS AND GETTERS --------------------

	void setNetwork(final BasicNetwork network) {
		this.link2weight = new HashMap<BasicLink, Double>();
		for (BasicLink link : network.getLinks()) {
			double weight = 0;
			for (int i = 0; i < this.attributes.size(); i++) {
				weight += this.coefficients.get(i)
						* Double.parseDouble(link.getAttr(this.attributes
								.get(i)));
			}
			this.link2weight.put(link, weight);
		}
	}

	void setLinkCostScale(final double linkCostScale) {
		this.linkCostScale = linkCostScale;
	}

	double getLinkCostScale() {
		return this.linkCostScale;
	}

	// void setNodeLoopScale(final double nodeLoopScale) {
	// this.nodeLoopScale = nodeLoopScale;
	// }

	// double getNodeLoopScale() {
	// return this.nodeLoopScale;
	// }

	// -------------------- IMPLEMENTATION OF LinkCost --------------------

	@Override
	public double getCost(final BasicLink link) {
		return this.link2weight.get(link);
	}

	// -------------------- IMPLEMENTATION OF MHWeight --------------------

	double logWeightWithoutCorrection(final MHPath path) {

		double pathCost = 0;
		for (BasicLink link : path.getLinks()) {
			pathCost += this.getCost(link);
		}

		return (-this.linkCostScale * pathCost);

		// final double nodeLoopCnt;
		// if (this.nodeLoopScale != 0.0) {
		// // we know that we are dealing with an inverted network here
		// final Set<String> originalNodeIds = new HashSet<String>();
		// originalNodeIds.add(path.getNodes().get(0)
		// .getAttr(NODE_ORIGINALFROMNODE_ID));
		// for (Node invertedNode : path.getNodes()) {
		// originalNodeIds.add(invertedNode
		// .getAttr(NODE_ORIGINALTONODE_ID));
		// }
		// nodeLoopCnt = (path.getLinks().size() + 2) - originalNodeIds.size();
		// } else {
		// nodeLoopCnt = 0.0;
		// }
		//
		// return (-this.linkCostScale * pathCost - this.nodeLoopScale
		// * nodeLoopCnt);
	}

	@Override
	public double logWeight(final MHPath path) {
		return (this.logWeightWithoutCorrection(path) - Math.log(path
				.pointCombinationSize()));
	}
}

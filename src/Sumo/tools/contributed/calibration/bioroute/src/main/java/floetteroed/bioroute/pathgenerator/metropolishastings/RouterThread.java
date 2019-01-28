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

import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;

import floetteroed.utilities.networks.basic.BasicNode;
import floetteroed.utilities.networks.shortestpaths.Router;


/**
 * 
 * @author Gunnar Flötteröd
 * 
 */
class RouterThread extends Thread {

	enum MODE {
		AB, BC
	};

	private final Router router;

	private final MHPath path;

	private final BasicNode insertNode;

	private final MODE mode;

	private List<BasicNode> result = null;

	RouterThread(final Router router, final MHPath path, final BasicNode nodeB,
			final MODE mode) {
		if (router == null || path == null || nodeB == null || mode == null) {
			throw new IllegalArgumentException();
		}
		this.router = router;
		this.path = path;
		this.insertNode = nodeB;
		this.mode = mode;
	}

	@Override
	public void run() {
		if (MODE.AB.equals(this.mode)) {
			final Set<BasicNode> excludedNodes = new HashSet<BasicNode>(this.path
					.getNodes().subList(0, this.path.getPoints().getA()));
			excludedNodes.addAll(this.path.getNodes().subList(
					this.path.getPoints().getC(), this.path.size()));
			final Map<BasicNode, Double> modifiedFwdCost = this.router
					.fwdCostWithoutExcludedNodes(this.path.getNodeA(),
							this.insertNode, this.router.getNetwork()
									.getNodes(), excludedNodes, null);
			this.result = this.router.bestRouteFwd(this.path.getNodeA(),
					this.insertNode, modifiedFwdCost);
		} else {
			final Set<BasicNode> excludedNodes = new HashSet<BasicNode>(this.path
					.getNodes().subList(0, this.path.getPoints().getA() + 1));
			excludedNodes.addAll(this.path.getNodes().subList(
					this.path.getPoints().getC() + 1, this.path.size()));
			final Map<BasicNode, Double> modifiedBwdCost = this.router
					.bwdCostWithoutExcludedNodes(this.path.getNodeC(),
							this.insertNode, this.router.getNetwork()
									.getNodes(), excludedNodes, null);
			this.result = this.router.bestRouteBwd(this.insertNode,
					this.path.getNodeC(), modifiedBwdCost);
		}
	}

	List<BasicNode> getResult() {
		return this.result;
	}

}

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
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.Set;

import floetteroed.utilities.Triple;
import floetteroed.utilities.networks.basic.BasicLink;
import floetteroed.utilities.networks.basic.BasicNode;
import floetteroed.utilities.networks.shortestpaths.Router;


/**
 * Path representation for the Metropolis-Hastings <code>PathGenerator</code>.
 * 
 * @author Gunnar Flötteröd
 * 
 */
public class MHPath {

	// TODO >>>>>>>>>>>>>>>>>>>> NEW STUFF >>>>>>>>>>>>>>>>>>>>
	//
	// private final Router router1;
	//
	// private final Router router2;
	//
	// private BasicNode lastInsertNode = null;
	//
	// private List<BasicNode> lastSegmentAB = null;
	//
	// private List<BasicNode> lastSegmentBC = null;
	//
	// private static long calls = 0;
	//
	// private static long recalcs = 0;
	//
	// private Tuple<List<BasicNode>, List<BasicNode>> getSegmentsABC(
	// final BasicNode insertNode) {
	// calls++;
	// // if (!insertNode.equals(this.lastInsertNode)) {
	// recalcs++;
	// final RouterThread threadAB = new RouterThread(this.router1, this,
	// insertNode, RouterThread.MODE.AB);
	// threadAB.start();
	// final RouterThread threadBC = new RouterThread(this.router2, this,
	// insertNode, RouterThread.MODE.BC);
	// threadBC.start();
	// // this.lastInsertNode = insertNode;
	// try {
	// threadAB.join();
	// this.lastSegmentAB = threadAB.getResult();
	// threadBC.join();
	// this.lastSegmentBC = threadBC.getResult();
	// } catch (InterruptedException e) {
	// throw new RuntimeException(e);
	// }
	// // }
	// if (calls % 100000 == 0) {
	// System.out.println(calls + "\tcalls");
	// System.out.println(recalcs + "\trecalculations");
	// System.out.println(Thread.activeCount() + "\trunning threads");
	// }
	//
	// return new Tuple<List<BasicNode>, List<BasicNode>>(this.lastSegmentAB,
	// this.lastSegmentBC);
	// }
	//
	// boolean isSpliceable() {
	// if (this.spliceable == null) {
	// final Tuple<List<BasicNode>, List<BasicNode>> pathsABC = this
	// .getSegmentsABC(this.getNodeB());
	// final List<BasicNode> pathAB = pathsABC.getA();
	// final List<BasicNode> pathBC = pathsABC.getB();
	// // final List<Node> pathAB =
	// // this.newSpliceSegmentAB(this.getNodeB());
	// if (!Router.equals(this.nodes, this.points.getA(),
	// this.points.getB(), pathAB, 0, pathAB.size() - 1)) {
	// this.spliceable = false;
	// } else {
	// // final List<Node> pathBC = this.newSpliceSegmentBC(this
	// // .getNodeB());
	// this.spliceable = (Router.equals(this.nodes,
	// this.points.getB(), this.points.getC(), pathBC, 0,
	// pathBC.size() - 1));
	// }
	// }
	// return this.spliceable;
	// }

	// TODO <<<<<<<<<<<<<<<<<<<< NEW STUFF <<<<<<<<<<<<<<<<<<<<

	// -------------------- CONSTANTS --------------------

	private final Router router;

	// -------------------- MEMBERS --------------------

	private final LinkedList<BasicNode> nodes;

	private Triple<Integer, Integer, Integer> points;

	// --------------- CACHED MEMBERS & ACCESS FUNCTIONS ---------------

	private List<BasicLink> links = null;

	private Double cost = null;

	private Boolean spliceable = null;

	public List<BasicLink> getLinks() {
		if (this.links == null) {
			this.links = Router.toLinkRoute(this.nodes);
		}
		return this.links;
	}

	double getCost() {
		if (this.cost == null) {
			this.cost = this.router.cost(this.getLinks());
		}
		return this.cost;
	}

	// TODO WORKING ORIGINAL
	boolean isSpliceable() {
		if (this.spliceable == null) {
			final List<BasicNode> pathAB = this.newSpliceSegmentAB(this
					.getNodeB());
			if (!Router.equals(this.nodes, this.points.getA(),
					this.points.getB(), pathAB, 0, pathAB.size() - 1)) {
				this.spliceable = false;
			} else {
				final List<BasicNode> pathBC = this.newSpliceSegmentBC(this
						.getNodeB());
				this.spliceable = (Router.equals(this.nodes,
						this.points.getB(), this.points.getC(), pathBC, 0,
						pathBC.size() - 1));
			}
		}
		return this.spliceable;
	}

	// -------------------- CONSTRUCTION --------------------

	MHPath(final LinkedList<BasicNode> nodes,
			final Triple<Integer, Integer, Integer> points, final Router router) {
		this.nodes = new LinkedList<BasicNode>(nodes);
		this.points = points;
		this.router = router;
		// this.router1 = router1;
		// this.router2 = router2;
		this.links = null;
		this.cost = null;
		this.spliceable = null;
	}

	MHPath(final MHPath parent) {
		// this(parent.nodes, parent.points, parent.router1, parent.router2);
		this(parent.nodes, parent.points, parent.router);
		this.links = (parent.links == null ? null : new LinkedList<BasicLink>(
				parent.links));
		this.cost = parent.cost;
		this.spliceable = parent.spliceable;
	}

	// -------------------- GETTERS --------------------

	// TODO WORKING ORIGINAL
	private LinkedList<BasicNode> newSpliceSegmentAB(final BasicNode insertNode) {
		final Set<BasicNode> excludedNodes = new HashSet<BasicNode>(
				this.nodes.subList(0, this.getPoints().getA()));
		excludedNodes.addAll(this.nodes.subList(this.getPoints().getC(),
				this.size()));
		final Map<BasicNode, Double> modifiedFwdCost = this.router
				.fwdCostWithoutExcludedNodes(this.getNodeA(), insertNode,
						this.router.getNetwork().getNodes(), excludedNodes,
						null);
		return this.router.bestRouteFwd(this.getNodeA(), insertNode,
				modifiedFwdCost);
	}

	// TODO WORKING ORIGINAL
	private LinkedList<BasicNode> newSpliceSegmentBC(final BasicNode insertNode) {
		final Set<BasicNode> excludedNodes = new HashSet<BasicNode>(
				this.nodes.subList(0, this.getPoints().getA() + 1));
		excludedNodes.addAll(this.nodes.subList(this.getPoints().getC() + 1,
				this.size()));
		final Map<BasicNode, Double> modifiedBwdCost = this.router
				.bwdCostWithoutExcludedNodes(this.getNodeC(), insertNode,
						this.router.getNetwork().getNodes(), excludedNodes,
						null);
		return this.router.bestRouteBwd(insertNode, this.getNodeC(),
				modifiedBwdCost);
	}

	int size() {
		return this.nodes.size();
	}

	long pointCombinationSize() {
		return 1l * this.size() * (this.size() - 1) * (this.size() - 2) / 6;
	}

	public List<BasicNode> getNodes() {
		return this.nodes;
	}

	BasicNode getNode(final int index) {
		return this.nodes.get(index);
	}

	BasicNode getNodeA() {
		return this.getNode(this.getPoints().getA());
	}

	BasicNode getNodeB() {
		return this.getNode(this.getPoints().getB());
	}

	BasicNode getNodeC() {
		return this.getNode(this.getPoints().getC());
	}

	Triple<Integer, Integer, Integer> getPoints() {
		return this.points;
	}

	boolean hasCycle() {
		return ((new HashSet<BasicNode>(this.nodes)).size() < this.nodes.size());
	}

	// -------------------- SETTERS --------------------

	void setPoints(final Triple<Integer, Integer, Integer> points) {
		if (!this.points.equals(points)) {
			this.points = points;
			this.spliceable = null;
		}
	}

	// -------------------- PATH MANIPULATIONS --------------------

	boolean insertDetour(final BasicNode nodeB) {

		/*
		 * (1) compute new path segments
		 */
		// >>>>> TODO WORKING ORIGINAL >>>>>
		final List<BasicNode> pathAB = this.newSpliceSegmentAB(nodeB);
		if (pathAB == null) {
			return false;
		}
		final List<BasicNode> pathBC = this.newSpliceSegmentBC(nodeB);
		if (pathBC == null) {
			return false;
		}
		// >>>>> TODO PARALLEL VERSION >>>>>
		// final Tuple<List<BasicNode>, List<BasicNode>> pathsABC = this
		// .getSegmentsABC(this.getNodeB());
		// final List<BasicNode> pathAB = pathsABC.getA();
		// if (pathAB == null) {
		// return false;
		// }
		// final List<BasicNode> pathBC = pathsABC.getB();
		// if (pathBC == null) {
		// return false;
		// }
		// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

		/*
		 * (2) build new path
		 */
		final LinkedList<BasicNode> newNodes = new LinkedList<BasicNode>();
		newNodes.addAll(this.nodes.subList(0, this.points.getA()));
		newNodes.addAll(pathAB);
		newNodes.addAll(pathBC.subList(1, pathBC.size() - 1));
		newNodes.addAll(this.nodes.subList(this.points.getC(),
				this.nodes.size()));
		/*
		 * (3) update indices
		 */
		int newB = this.points.getA() + (pathAB.size() - 1);
		int newC = newB + (pathBC.size() - 1);
		final Triple<Integer, Integer, Integer> newPoints = new Triple<Integer, Integer, Integer>(
				this.points.getA(), newB, newC);
		/*
		 * (4) clear/update internal cache
		 */
		this.links = null;
		this.cost = null;
		this.spliceable = true; // because this was created by a splice
		/*
		 * (5) update further data structures
		 */
		this.nodes.clear();
		this.nodes.addAll(newNodes);
		this.points = newPoints;

		return true;
	}

	// -------------------- OVERRIDING OF Object --------------------

	@Override
	public String toString() {
		final StringBuffer result = new StringBuffer();
		for (int i = 0; i < this.nodes.size() - 1; i++) {
			result.append(this.nodes.get(i).getId());
			result.append(" ");
		}
		result.append(this.nodes.getLast().getId());
		return result.toString();
	}
}

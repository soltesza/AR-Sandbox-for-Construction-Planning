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

import static java.lang.Math.log;

import java.util.LinkedHashMap;
import java.util.LinkedList;
import java.util.Map;
import java.util.Random;

import floetteroed.utilities.Triple;
import floetteroed.utilities.math.metropolishastings.MHProposal;
import floetteroed.utilities.math.metropolishastings.MHTransition;
import floetteroed.utilities.networks.basic.BasicNode;
import floetteroed.utilities.networks.shortestpaths.Router;


/**
 * Proposal distribution for the Metropolis-Hastings <code>PathGenerator</code>.
 * 
 * @author Gunnar Flötteröd
 * 
 */
class MHPathProposal implements MHProposal<MHPath> {

	// -------------------- MEMBERS --------------------

	private final BasicNode origin;

	private final BasicNode destination;

	private final Router router;

	// private final Router router2;

	private final Random rnd;

	// TODO NEW
	final double spliceProbability;

	// TODO NEW
	final Map<BasicNode, Double> proposalProbabilities;

	// -------------------- CONSTRUCTION --------------------

	MHPathProposal(final BasicNode origin, final BasicNode destination,
			final Router router, final double spliceProbability,
			final Map<BasicNode, Double> proposalProbabilities, final Random rnd) {
		if (origin == null) {
			throw new IllegalArgumentException("origin is null");
		}
		if (destination == null) {
			throw new IllegalArgumentException("destination is null");
		}
		if (router == null) {
			throw new IllegalArgumentException("router is null");
		}
		// if (router1 == null || router2 == null) {
		// throw new IllegalArgumentException("router is null");
		// }
		if (rnd == null) {
			throw new IllegalArgumentException("rnd is null");
		}
		this.origin = origin;
		this.destination = destination;
		this.router = router;
		this.spliceProbability = spliceProbability;
		this.proposalProbabilities = new LinkedHashMap<BasicNode, Double>(
				proposalProbabilities);
		this.rnd = rnd;
	}

	// -------------------- INTERNALS --------------------

	static Triple<Integer, Integer, Integer> drawPoints(final int n,
			final Random rnd) {
		/*
		 * (1) check
		 */
		if (n < 3) {
			throw new RuntimeException("less than three alternatives!");
		}
		/*
		 * (2) draw three disjoint numbers
		 */
		int u1 = rnd.nextInt(n);
		int u2 = rnd.nextInt(n - 1);
		if (u2 >= u1) {
			u2++;
		}
		int u3 = rnd.nextInt(n - 2);
		if (u3 >= Math.min(u1, u2)) {
			u3++;
		}
		if (u3 >= Math.max(u1, u2)) {
			u3++;
		}
		/*
		 * (3) return sorted numbers
		 */
		if (u1 < u2 && u1 < u3) {
			if (u2 < u3) {
				return new Triple<Integer, Integer, Integer>(u1, u2, u3);
			} else {
				return new Triple<Integer, Integer, Integer>(u1, u3, u2);
			}
		} else if (u2 < u1 && u2 < u3) {
			if (u1 < u3) {
				return new Triple<Integer, Integer, Integer>(u2, u1, u3);
			} else {
				return new Triple<Integer, Integer, Integer>(u2, u3, u1);
			}
		} else {
			if (u1 < u2) {
				return new Triple<Integer, Integer, Integer>(u3, u1, u2);
			} else {
				return new Triple<Integer, Integer, Integer>(u3, u2, u1);
			}
		}
	}

	private double transitionLogProb(final MHPath fromRoute,
			final MHPath toRoute) {

		if (!fromRoute.getNodes().equals(toRoute.getNodes())) {
			/*
			 * different routes -- a feasible SPLICE has occurred
			 */
			return log(this.spliceProbability)
					+ log(this.proposalProbabilities.get(toRoute.getNodeB()));
		} else {
			/*
			 * identical routes (this means that indices and nodes are equiv.)
			 */
			if (fromRoute.getPoints().getA().equals(toRoute.getPoints().getA())
					&& fromRoute.getPoints().getC()
							.equals(toRoute.getPoints().getC())) {
				/*
				 * identical routes, identical points a and c
				 */
				if (fromRoute.getPoints().getB()
						.equals(toRoute.getPoints().getB())) {
					/*
					 * identical routes, identical points a, b, and c
					 */
					return 0.0;
				} else {
					/*
					 * identical routes, identical points a and c, different
					 * point b -- a feasible SPLICE must have occurred because
					 * shuffle-b-only is not allowed
					 */
					return log(this.spliceProbability)
							+ log(this.proposalProbabilities.get(toRoute
									.getNodeB()));
				}
			} else {
				/*
				 * identical routes, different points a or c -- a SHUFFLE must
				 * have occurred
				 */
				final long forbiddenSize = fromRoute.getPoints().getC()
						- fromRoute.getPoints().getA() - 2;
				if (fromRoute.isSpliceable()) {
					return log(1.0 - this.spliceProbability)
							- log(fromRoute.pointCombinationSize()
									- forbiddenSize);
				} else {
					return -log(fromRoute.pointCombinationSize()
							- forbiddenSize);
				}
			}
		}
	}

	// -------------------- IMPLEMENTATION OF MHProposal --------------------

	@Override
	public MHPath newInitialState() {
		final LinkedList<BasicNode> nodes = this.router.bestRoute(this.origin,
				this.destination);
		final Triple<Integer, Integer, Integer> points = drawPoints(
				nodes.size(), this.rnd);
		return new MHPath(nodes, points, this.router);
	}

	// TODO NEW
	private BasicNode drawInsertNode() {
		final double u = this.rnd.nextDouble();
		double probabilitySum = 0;
		for (Map.Entry<BasicNode, Double> entry : this.proposalProbabilities
				.entrySet()) {
			probabilitySum += entry.getValue();
			if (probabilitySum >= u) {
				return entry.getKey();
			}
		}
		return null;
	}

	@Override
	public MHTransition<MHPath> newTransition(final MHPath fromRoute) {
		final MHPath toRoute;
		if (this.rnd.nextDouble() < this.spliceProbability
				&& fromRoute.isSpliceable()) {
			/*
			 * SPLICE
			 */
			final BasicNode insertNode = this.drawInsertNode();
			if ((insertNode == null)
					|| fromRoute.getNodes()
							.subList(0, fromRoute.getPoints().getA() + 1)
							.contains(insertNode)
					|| fromRoute
							.getNodes()
							.subList(fromRoute.getPoints().getC(),
									fromRoute.getNodes().size())
							.contains(insertNode)) {
				toRoute = new MHPath(fromRoute);
			} else {
				final MHPath proposalRoute = new MHPath(fromRoute);
				if (proposalRoute.insertDetour(insertNode)) {
					if (proposalRoute.hasCycle()) {
						toRoute = new MHPath(fromRoute);
					} else {
						toRoute = proposalRoute;

						// System.out.println("SPLICE with proposal node "
						// + insertNode.getId()
						// + " and new proposal route " + toRoute);

					}
				} else {
					toRoute = new MHPath(fromRoute);
				}
			}
		} else {
			/*
			 * SHUFFLE
			 * 
			 * It is not allowed to only shuffle B. The loop below will
			 * eventually terminate because there is always the chance to
			 * shuffle such that A, B, and C are the same as before.
			 */
			toRoute = new MHPath(fromRoute);
			Triple<Integer, Integer, Integer> newPoints;
			do {
				newPoints = drawPoints(toRoute.size(), this.rnd);
			} while (fromRoute.getPoints().getA().equals(newPoints.getA())
					&& fromRoute.getPoints().getC().equals(newPoints.getC())
					&& !fromRoute.getPoints().getB().equals(newPoints.getB()));
			toRoute.setPoints(newPoints);

			// System.out.println("SHUFFLE with new points " + newPoints);

		}
		final double fwdLogProb = this.transitionLogProb(fromRoute, toRoute);
		final double bwdLogProb = this.transitionLogProb(toRoute, fromRoute);
		return new MHTransition<MHPath>(fromRoute, toRoute, fwdLogProb,
				bwdLogProb);
	}
}

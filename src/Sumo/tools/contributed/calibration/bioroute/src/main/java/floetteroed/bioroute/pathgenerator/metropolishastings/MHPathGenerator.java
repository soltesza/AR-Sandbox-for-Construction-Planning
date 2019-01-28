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

import java.util.LinkedHashMap;
import java.util.Map;
import java.util.Random;

import floetteroed.bioroute.BiorouteRunner;
import floetteroed.bioroute.PathGenerator;
import floetteroed.bioroute.PathWriter;
import floetteroed.utilities.config.Config;
import floetteroed.utilities.math.MathHelpers;
import floetteroed.utilities.math.metropolishastings.MHAlgorithm;
import floetteroed.utilities.networks.basic.BasicNetwork;
import floetteroed.utilities.networks.basic.BasicNode;
import floetteroed.utilities.networks.shortestpaths.Router;

/**
 * A Metropolis-Hastings code for path sampling.
 * 
 * @see <a
 *      href="http://transp-or.epfl.ch/documents/technicalReports/FloeBier11.pdf"
 *      > Flötteröd & Bierlaire (2011)<a/>
 * 
 * @author Gunnar Flötteröd
 * 
 */
public class MHPathGenerator implements PathGenerator {

	// -------------------- CONSTANTS --------------------

	// DEFAULT PARAMETER VALUES

	public static final int DEFAULT_MSGINTERVAL = Integer.MAX_VALUE;

	public static final int DEFAULT_BURNINITERATIONS = 0;

	public static final int DEFAULT_SAMPLEINTERVAL = 1;

	public static final double DEFAULT_SPLICEPROBABILITY = 0.5;

	public static final double DEFAULT_PROPOSALSCALEFACTOR = 1.0;

	// CONFIG ELEMENTS

	public static final String RANDOMSEED_ELEMENT = "randomseed";

	public static final String MSGINTERVAL_ELEMENT = "msginterval";

	public static final String TOTALITERATIONS_ELEMENT = "totaliterations";

	public static final String SAMPLEINTERVAL_ELEMENT = "sampleinterval";

	public static final String RELATIVECOSTSCALE_ELEMENT = "relativecostscale";

	public static final String RELATIVECOSTOFFSET_ELEMENT = "relativecostoffset";

	public static final String SPLICEPROBABILITY_ELEMENT = "spliceprobability";

	public static final String PROPOSALSCALEFACTOR_ELEMENT = "proposalscalefactor";

	// -------------------- MEMBERS --------------------

	// CONFIGURATION

	private Integer msgInterval = null;

	// private Long randomSeed = null;

	private Integer totalIterations = null;

	private Integer sampleInterval = null;

	private Double relativeCostScale = null;

	private Double relativeCostOffset = null;

	private Double spliceProbability = null;

	private Double proposalScaleFactor = null;

	// RUNTIME

	private Random rnd = null;

	private MHLinkAndPathCost linkAndPathCost = null;

	// private Network invertedNetwork = null;
	private BasicNetwork network = null;

	private PathWriter pathWriter = null;

	// -------------------- CONSTRUCTION --------------------

	public MHPathGenerator() {
		// no-argument constructor for reflective instantiation
	}

	// TODO NEW
	MHLinkAndPathCost newConfiguredLinkAndPathCost(final Config config) {
		// final MHLinkAndPathCost result = new MHLinkAndPathCost();
		final MHLinkAndPathCost result = this.newLinkAndPathCost();
		result.configure(config);
		return result;
	}

	protected MHLinkAndPathCost newLinkAndPathCost() {
		return new MHLinkAndPathCost();
	}
	
	protected MHPath initialState(final BasicNode originNode,
			final BasicNode destinationNode, final Router router) {
		return null;
	}


	// -------------------- IMPLEMENTATION OF PathGenerator --------------------

	@Override
	public void configure(final Config config) {

		this.msgInterval = MathHelpers.parseInteger(config.get(
				BiorouteRunner.PATHGENERATOR_ELEMENT, MSGINTERVAL_ELEMENT));
		if (this.msgInterval == null) {
			this.msgInterval = DEFAULT_MSGINTERVAL;
		}

		// TODO NEW
		Long randomSeed = MathHelpers.parseLong(config.get(
				BiorouteRunner.PATHGENERATOR_ELEMENT, RANDOMSEED_ELEMENT));
		if (randomSeed == null) {
			randomSeed = (new Random()).nextLong();
		}
		this.rnd = new Random(randomSeed);
		System.out.println("random seed = " + randomSeed);
		// if (this.randomSeed == null) {
		// this.rnd = new Random();
		// } else {
		// this.rnd = new Random(this.randomSeed);
		// }

		this.totalIterations = MathHelpers.parseInteger(config.get(
				BiorouteRunner.PATHGENERATOR_ELEMENT, TOTALITERATIONS_ELEMENT));
		if (this.totalIterations == null) {
			throw new IllegalArgumentException(
					"total iterations is not specified");
		}

		this.sampleInterval = MathHelpers.parseInteger(config.get(
				BiorouteRunner.PATHGENERATOR_ELEMENT, SAMPLEINTERVAL_ELEMENT));
		if (this.sampleInterval == null) {
			this.sampleInterval = DEFAULT_SAMPLEINTERVAL;
		}

		// may be null:
		this.relativeCostScale = MathHelpers.parseDouble(config
				.get(BiorouteRunner.PATHGENERATOR_ELEMENT,
						RELATIVECOSTSCALE_ELEMENT));

		// may be null
		this.relativeCostOffset = MathHelpers.parseDouble(config.get(
				BiorouteRunner.PATHGENERATOR_ELEMENT,
				RELATIVECOSTOFFSET_ELEMENT));

		this.spliceProbability = MathHelpers.parseDouble(config
				.get(BiorouteRunner.PATHGENERATOR_ELEMENT,
						SPLICEPROBABILITY_ELEMENT));
		if (this.spliceProbability == null) {
			this.spliceProbability = DEFAULT_SPLICEPROBABILITY;
		}

		this.proposalScaleFactor = MathHelpers.parseDouble(config.get(
				BiorouteRunner.PATHGENERATOR_ELEMENT,
				PROPOSALSCALEFACTOR_ELEMENT));
		if (this.proposalScaleFactor == null) {
			this.proposalScaleFactor = DEFAULT_PROPOSALSCALEFACTOR;
		}

		// TODO NEW
		this.linkAndPathCost = this.newConfiguredLinkAndPathCost(config);
	}

	@Override
	public void setNetwork(final BasicNetwork network) {
		this.network = network;
		// this.invertedNetwork = NetworkInverter.newInvertedNetwork(network);
	}

	@Override
	public void setPathWriter(final PathWriter pathWriter) {
		this.pathWriter = pathWriter;
	}

	@Override
	public void run(final BasicNode originNode, final BasicNode destinationNode) {

		/*
		 * (1) switch to a fresh copy of the original network
		 * 
		 * TODO CHANGED
		 */
		this.linkAndPathCost.setNetwork(this.network);
		// final Network invertedNetwork = new Network(this.invertedNetwork);
		// this.linkAndPathCost.setNetwork(invertedNetwork);

		/*
		 * (2) identify OD pair in inverted network
		 * 
		 * TODO CHANGED
		 */
		// final BasicNode originNode = originLink.getToNode();
		// final BasicNode destinationNode = destinationLink.getFromNode();
		// final Node invertedOriginNode = invertedNetwork.getNode(originLink
		// .getId());
		// final Node invertedDestinationNode = invertedNetwork
		// .getNode(destinationLink.getId());

		/*
		 * (3) compute shortest path link cost and new scale if it is defined in
		 * relative terms
		 * 
		 * TODO CHANGED
		 */
		final Router router = new Router(this.network, this.linkAndPathCost);
		if (this.relativeCostScale != null) {
			final double linkCostSP = router.fwdCost(originNode,
					destinationNode).get(destinationNode);
			this.linkAndPathCost
					.setLinkCostScale(Math.log(2.0)
							/ (linkCostSP + (this.relativeCostOffset != null ? this.relativeCostOffset
									: 0.0)) / (this.relativeCostScale - 1));
		}
		// final Router router = new Router(this.invertedNetwork,
		// this.linkAndPathCost);
		// if (this.relativeCostScale != null) {
		// final double nodeLoopScale = this.linkAndPathCost
		// .getNodeLoopScale();
		// this.linkAndPathCost.setNodeLoopScale(0.0);
		// final double linkCostSP = router.fwdCost(invertedOriginNode,
		// invertedDestinationNode).get(invertedDestinationNode);
		// this.linkAndPathCost.setNodeLoopScale(nodeLoopScale);
		// this.linkAndPathCost.setLinkCostScale(Math.log(2.0) / linkCostSP
		// / (this.relativeCostScale - 1));
		// }

		/*
		 * (4) compute fixed SPLICE proposal probabilities
		 * 
		 * TODO CHANGED
		 */
		final Map<BasicNode, Double> proposalProbabilities = new LinkedHashMap<BasicNode, Double>();
		final Map<BasicNode, Double> fwdCost = router.fwdCost(originNode);
		final Map<BasicNode, Double> bwdCost = router.bwdCost(destinationNode);
		double minCost = Double.POSITIVE_INFINITY;
		for (BasicNode node : this.network.getNodes()) {
			final double cost;
			if ((fwdCost.get(node) != null) && (bwdCost.get(node) != null)) {
				cost = fwdCost.get(node) + bwdCost.get(node);
			} else {
				cost = Double.POSITIVE_INFINITY;
			}
			minCost = Math.min(minCost, cost);
			proposalProbabilities.put(node, cost);
		}
		double weightSum = 0;
		for (BasicNode node : this.network.getNodes()) {
			final double cost = proposalProbabilities.get(node);
			final double weight;
			if (Double.isInfinite(cost)) {
				weight = 0;
			} else {
				weight = Math.exp(this.linkAndPathCost.getLinkCostScale()
						* this.proposalScaleFactor * (-cost + minCost));
				weightSum += weight;
			}
			proposalProbabilities.put(node, weight);
		}
		for (BasicNode node : this.network.getNodes()) {
			final double weight = proposalProbabilities.get(node);
			proposalProbabilities.put(node, weight / weightSum);
		}
		// final Map<Node, Double> proposalProbabilities = new
		// LinkedHashMap<Node, Double>();
		// final Map<Node, Double> fwdCost = router.fwdCost(invertedOriginNode);
		// final Map<Node, Double> bwdCost = router
		// .bwdCost(invertedDestinationNode);
		// double minCost = Double.POSITIVE_INFINITY;
		// for (Node node : this.invertedNetwork.getNodes()) {
		// final double cost = fwdCost.get(node) + bwdCost.get(node);
		// minCost = Math.min(minCost, cost);
		// proposalProbabilities.put(node, cost);
		// }
		// double weightSum = 0;
		// for (Node node : this.invertedNetwork.getNodes()) {
		// final double cost = proposalProbabilities.get(node);
		// final double weight = Math.exp(this.linkAndPathCost
		// .getLinkCostScale()
		// * this.proposalScaleFactor
		// * (-cost + minCost));
		// weightSum += weight;
		// proposalProbabilities.put(node, weight);
		// }
		// for (Node node : this.invertedNetwork.getNodes()) {
		// final double weight = proposalProbabilities.get(node);
		// proposalProbabilities.put(node, weight / weightSum);
		// // System.out.println("weight(" + node.getId() + ") = "
		// // + proposalProbabilities.get(node));
		// }

		/*
		 * (5) run the algorithm
		 * 
		 * TODO CHANGED
		 */
		final MHPathProposal_NEW proposal = new MHPathProposal_NEW(originNode,
				destinationNode, router, this.spliceProbability,
				proposalProbabilities, this.rnd);
		final MHAlgorithm<MHPath> algo = new MHAlgorithm<MHPath>(proposal,
				this.linkAndPathCost, this.rnd);
		algo.setMsgInterval(this.msgInterval);
		algo.addStateProcessor(new MHPathWriterWrapper(this.pathWriter,
				this.sampleInterval, this.linkAndPathCost));

		algo.setInitialState(this.initialState(originNode, destinationNode, router));
		
		algo.run(this.totalIterations);
		System.out.println("total MH runtime = " + algo.getLastCompTime_ms());
		// final MHPathProposal proposal = new
		// MHPathProposal(invertedOriginNode,
		// invertedDestinationNode, router, this.spliceProbability,
		// proposalProbabilities, this.rnd);
		// final MHAlgorithm<MHPath> algo = new MHAlgorithm<MHPath>(proposal,
		// this.linkAndPathCost, this.rnd);
		// algo.setMsgInterval(this.msgInterval);
		// algo.addStateProcessor(new MHPathWriterWrapper(this.pathWriter,
		// this.sampleInterval, this.linkAndPathCost));
		// algo.run(this.totalIterations);
		// System.out.println("total MH runtime = " +
		// algo.getLastCompTime_ms());
	}
}

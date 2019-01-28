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
package floetteroed.bioroute;

import java.util.ArrayList;
import java.util.List;

import floetteroed.bioroute.utilities.NetworkInstantiator;
import floetteroed.utilities.ErrorMsgPrinter;
import floetteroed.utilities.Tuple;
import floetteroed.utilities.config.Config;
import floetteroed.utilities.config.ConfigReader;
import floetteroed.utilities.config.ConfigurableInstantiator;
import floetteroed.utilities.networks.basic.BasicLink;
import floetteroed.utilities.networks.basic.BasicNetwork;
import floetteroed.utilities.networks.basic.BasicNode;

/**
 * The main class for running BIOROUTE. It
 * <ol>
 * <li>reads the configuration XML file;
 * <li>instantiates the indicated classes;
 * <li>configures these classes;
 * <li>executes them.
 * </ol>
 * 
 * @author Gunnar Flötteröd
 * 
 */
public class BiorouteRunner {

	// -------------------- CONSTANTS --------------------

	public static final String BIOROUTE_ELEMENT = "bioroute";

	public static final String CLASSNAME_ELEMENT = "classname";

	public static final String NETWORKLOADER_ELEMENT = "networkloader";

	public static final String NETWORKPREPROCESSOR_ELEMENT = "networkpreprocessor";

	public static final String ODPAIRS_ELEMENT = "odpairs";

	public static final String ODPAIR_ELEMENT = "odpair";

	public static final String ORIGIN_ELEMENT = "origin";

	public static final String DESTINATION_ELEMENT = "destination";

	public static final String PATHGENERATOR_ELEMENT = "pathgenerator";

	public static final String PATHWRITER_CONFIG_ELEMENT = "pathwriter";

	// -------------------- IMPLEMENTATION --------------------

	/**
	 * Private constructor -- this class cannot be instantiated.
	 */
	private BiorouteRunner() {
	}

	// -------------------- IMPLEMENTATION --------------------

	/**
	 * Parses the <code>BIOROUTE_ELEMENT.ODPAIRS_ELEMENT</code> subtree of the
	 * <em>full</em> XML configuration <code>config</code> instance for
	 * origin/destination pair specifications.
	 * 
	 * @param net
	 *            the network
	 * @param config
	 *            the (full) configuration
	 * @return a list of origin/destination node reference tuples
	 */
	protected static List<Tuple<BasicNode, BasicNode>> extractOdPairs(
			final BasicNetwork net, final Config config) {
		final int odPairCnt = config.getList(BIOROUTE_ELEMENT, ODPAIRS_ELEMENT,
				ODPAIR_ELEMENT, ORIGIN_ELEMENT).size();
		final List<Tuple<BasicNode, BasicNode>> result = new ArrayList<Tuple<BasicNode, BasicNode>>(
				odPairCnt);
		for (int i = 0; i < odPairCnt; i++) {
			final String origin = config.get(i, BIOROUTE_ELEMENT,
					ODPAIRS_ELEMENT, ODPAIR_ELEMENT, ORIGIN_ELEMENT);
			final String destination = config.get(i, BIOROUTE_ELEMENT,
					ODPAIRS_ELEMENT, ODPAIR_ELEMENT, DESTINATION_ELEMENT);

			if (origin.equals(destination)) {
				System.out.println("Skipping O/D pair " + origin + "/"
						+ destination
						+ " because origin and destination are identical.");
			} else {
				final BasicLink shortcut = net.getLink(origin, destination);
				if (shortcut == null) {
					result.add(new Tuple<BasicNode, BasicNode>(net
							.getNode(origin), net.getNode(destination)));
				} else {
					System.out.println("Skipping O/D pair " + origin + "/"
							+ destination
							+ " because it is directly connected by link "
							+ shortcut.getId() + ".");
				}
			}
		}
		return result;
	}

	// private static Tuple<BasicLink, BasicLink> getStartEndLinksForNodeIds(
	// final BasicNetwork net, final String origin,
	// final String destination) {
	// BasicLink startLink = null;
	// BasicLink endLink = null;
	// for (BasicLink link : net.getLinks()) {
	// if (link.getFromNode().getId().equals(origin)) {
	// startLink = link;
	// }
	// if (link.getToNode().getId().equals(destination)) {
	// endLink = link;
	// }
	// }
	// if (startLink == null) {
	// System.err.println("origin node " + origin
	// + " has no outgoing link");
	// }
	// if (endLink == null) {
	// System.err.println("destination node " + destination
	// + " has no ingoing link");
	// }
	// if (startLink != null && endLink != null) {
	// return new Tuple<BasicLink, BasicLink>(startLink, endLink);
	// } else {
	// return null;
	// }
	// }

	// TODO running original
	// protected static List<Tuple<BasicLink, BasicLink>> extractOdPairs(
	// final BasicNetwork net, final Config config) {
	// final int odPairCnt = config.getList(BIOROUTE_ELEMENT, ODPAIRS_ELEMENT,
	// ODPAIR_ELEMENT, ORIGIN_ELEMENT).size();
	// final List<Tuple<BasicLink, BasicLink>> result = new
	// ArrayList<Tuple<BasicLink, BasicLink>>(
	// odPairCnt);
	// for (int i = 0; i < odPairCnt; i++) {
	// final String origin = config.get(i, BIOROUTE_ELEMENT,
	// ODPAIRS_ELEMENT, ODPAIR_ELEMENT, ORIGIN_ELEMENT);
	// final String destination = config.get(i, BIOROUTE_ELEMENT,
	// ODPAIRS_ELEMENT, ODPAIR_ELEMENT, DESTINATION_ELEMENT);
	// result.add(new Tuple<BasicLink, BasicLink>(net.getLink(origin), net
	// .getLink(destination)));
	// }
	// return result;
	// }

	// -------------------- MAIN-FUNCTION --------------------

	/**
	 * The BIOROUTE main function.
	 * 
	 * @params args command-line parameters; should contain exactly one entry
	 *         indicating the config XML file
	 */
	public static void main(String[] args) {

		try {

			System.out.println("STARTED..");

			/*
			 * >>>>> TODO TEST >>>>>
			 */
			// System.out.print("Waiting until one processor is assigned ... ");
			// int proc = 0;
			// Runtime rt;
			// do {
			// rt = Runtime.getRuntime();
			// if (rt.availableProcessors() != proc) {
			// proc = rt.availableProcessors();
			// System.out.print("(available=" + proc+") ");
			// }
			// } while (rt.availableProcessors() > 1);
			// System.out.println("OK");
			/*
			 * <<<<< TODO TEST <<<<<
			 */

			/*
			 * (1) Extract single command line parameter.
			 */
			if (args == null || args.length == 0) {
				System.err.println("name of configuration file is missing");
				System.exit(-1);
			}
			final String configFileName = args[0];

			/*
			 * (2) Load configuration.
			 */
			final Config config = (new ConfigReader()).read(configFileName);

			/*
			 * (3) Instantiate and run the network loader and preprocessor.
			 */
			final BasicNetwork network = NetworkInstantiator.loadNetwork(
					config, BIOROUTE_ELEMENT, NETWORKLOADER_ELEMENT,
					CLASSNAME_ELEMENT);
			NetworkInstantiator.preprocessNetwork(network, config,
					BIOROUTE_ELEMENT, NETWORKPREPROCESSOR_ELEMENT,
					CLASSNAME_ELEMENT);

			/*
			 * (4) Instantiate the path writer.
			 */
			final PathWriter pathWriter = (PathWriter) ConfigurableInstantiator
					.newConfiguredInstance(config, BIOROUTE_ELEMENT,
							PATHWRITER_CONFIG_ELEMENT, CLASSNAME_ELEMENT);

			/*
			 * (5) Instantiate and initialize the path generator.
			 */
			final PathGenerator pathGenerator = (PathGenerator) ConfigurableInstantiator
					.newConfiguredInstance(config, BIOROUTE_ELEMENT,
							PATHGENERATOR_ELEMENT, CLASSNAME_ELEMENT);
			pathGenerator.setNetwork(network);
			pathGenerator.setPathWriter(pathWriter);

			/*
			 * (6) Run the whole thing.
			 */
			int i = 0;
			pathWriter.open();
			List<Tuple<BasicNode, BasicNode>> odPairs = extractOdPairs(network,
					config);
			for (Tuple<BasicNode, BasicNode> odPair : odPairs) {
				System.out.println("processing od pair #" + (++i) + "/"
						+ odPairs.size() + " from link "
						+ odPair.getA().getId() + " to link "
						+ odPair.getB().getId());
				pathWriter.startOdPair(odPair.getA().getId(), odPair.getB()
						.getId());
				// if (odPair.getA().getToNode()
				// .equals(odPair.getB().getFromNode())) {
				// System.err.println("skipping adjacent links");
				// } else {
				pathGenerator.run(odPair.getA(), odPair.getB());
				// }
				pathWriter.endOdPair();
			}
			pathWriter.close();

			/*
			 * >>>>> TODO EXPERIMENTAL >>>>>
			 */
			// final String correlationFile = "corr.txt";
			// AnalysisRunner.main(new String[] { "CORRELATION", "-configfile",
			// "config.xml", "-maxdistance", "100", "-resultfile",
			// correlationFile });
			// AnalysisRunner.main(new String[] { "STATIONARITY",
			// "-correlationfile", correlationFile, "-slopethreshold",
			// "1e-3", "-windowsize", "10" });
			/*
			 * <<<<< TODO EXPERIMENTAL <<<<<
			 */

			System.out.println("..DONE");

		} catch (Exception e) {
			ErrorMsgPrinter.toStdOut(e);
			ErrorMsgPrinter.toErrOut(e);
		}

	}
}

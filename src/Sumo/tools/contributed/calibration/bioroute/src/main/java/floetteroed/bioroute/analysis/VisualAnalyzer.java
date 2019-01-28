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
package floetteroed.bioroute.analysis;

import static floetteroed.bioroute.BiorouteRunner.NETWORKLOADER_ELEMENT;
import static floetteroed.bioroute.networkloader.SUMOLoader.CONNECTIONFILE_ELEMENT;
import static floetteroed.bioroute.networkloader.SUMOLoader.EDGEFILE_ELEMENT;
import static floetteroed.bioroute.networkloader.SUMOLoader.NODEFILE_ELEMENT;
import static floetteroed.utilities.networks.containerloaders.MATSimNetworkContainerLoader.MATSIM_NETWORK_TYPE;
import static floetteroed.utilities.networks.containerloaders.OpenStreetMapNetworkContainerLoader.OPENSTREETMAP_NETWORK_TYPE;
import static floetteroed.utilities.networks.containerloaders.SUMONetworkContainerLoader.SUMO_NETWORK_TYPE;
import static floetteroed.utilities.visualization.NetvisFromFileRunner.ANTIALIASING_ELEMENT;
import static floetteroed.utilities.visualization.NetvisFromFileRunner.COLOR_ELEMENT;
import static floetteroed.utilities.visualization.NetvisFromFileRunner.DELAY_ELEMENT;
import static floetteroed.utilities.visualization.NetvisFromFileRunner.LINKDATAFILE_ELEMENT;
import static floetteroed.utilities.visualization.NetvisFromFileRunner.LINKWIDTH_ELEMENT;
import static floetteroed.utilities.visualization.NetvisFromFileRunner.LOGO_ELEMENT;
import static floetteroed.utilities.visualization.NetvisFromFileRunner.MULTILANE_ELEMENT;
import static floetteroed.utilities.visualization.NetvisFromFileRunner.NETVISCONFIG_ELEMENT;
import static floetteroed.utilities.visualization.NetvisFromFileRunner.NETWORKTYPE_ELEMENT;
import static floetteroed.utilities.visualization.NetvisFromFileRunner.SHOWLINKLABELS_ELEMENT;
import static floetteroed.utilities.visualization.NetvisFromFileRunner.SHOWNODELABELS_ELEMENT;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.PrintWriter;
import java.util.List;

import org.xml.sax.Attributes;

import floetteroed.bioroute.networkloader.AbstractNetworkLoader;
import floetteroed.bioroute.networkloader.SUMOLoader;
import floetteroed.utilities.DynamicData;
import floetteroed.utilities.DynamicDataXMLFileIO;
import floetteroed.utilities.config.Config;
import floetteroed.utilities.networks.basic.BasicLink;
import floetteroed.utilities.networks.basic.BasicNetwork;
import floetteroed.utilities.networks.basic.BasicNode;
import floetteroed.utilities.visualization.NetvisFromFileRunner;

/**
 * Creates a graphical representation of the paths in a given sample.
 * 
 * @author Gunnar Flötteröd
 * 
 */
class VisualAnalyzer extends DynamicDataXMLFileIO<BasicLink> implements
		PathHandler {

	// -------------------- CONSTANTS --------------------

	private static final long serialVersionUID = 1L;

	private final BasicNetwork network;

	private final Config networkLoaderConfig;

	// -------------------- MEMBERS --------------------

	private String toConfigFile = null;

	private String toDataFile = null;

	private DynamicData<BasicLink> data = new DynamicData<BasicLink>(0, 1, 100);

	// private Set<BasicLink> allLinks = new HashSet<BasicLink>();

	private int nextBin = 0;

	// -------------------- CONSTRUCTION --------------------

	VisualAnalyzer(final BasicNetwork network, final Config networkLoaderConfig) {
		if (network == null) {
			throw new IllegalArgumentException("network is null");
		}
		if (networkLoaderConfig == null) {
			throw new IllegalArgumentException("network loader config is null");
		}
		this.network = network;
		this.networkLoaderConfig = networkLoaderConfig;
	}

	// -------------------- IMPLEMENTATION --------------------

	void run(final String fromFile, final String toConfigFile,
			final String toDataFile) throws IOException {
		if (fromFile == null) {
			throw new IllegalArgumentException("from file is null");
		}
		if (toConfigFile == null) {
			throw new IllegalArgumentException("visual config file is null");
		}
		this.toConfigFile = toConfigFile;
		this.toDataFile = toDataFile;

		final PathXMLParser parser = new PathXMLParser(this.network);
		parser.parse(fromFile, this);
	}

	// -------------------- IMPLEMENTATION OF PathHandler --------------------

	@Override
	public void startPaths(final Attributes attrs) {
		/*
		 * (1) initialize internally
		 */
		this.data = new DynamicData<BasicLink>(0, 1, 100);
		this.nextBin = 0;
		/*
		 * (2) write config file
		 */
		try {
			final PrintWriter writer = new PrintWriter(this.toConfigFile);
			writer.println("<" + NETVISCONFIG_ELEMENT + ">");

			writer.println("  <" + LOGO_ELEMENT + " value = \"\"/>");
			writer.println("  <" + DELAY_ELEMENT + " value = \"250\"/>");
			writer.println("  <" + LINKWIDTH_ELEMENT + " value = \"1\"/>");
			writer.println("  <" + SHOWNODELABELS_ELEMENT
					+ " value = \"false\"/>");
			writer.println("  <" + SHOWLINKLABELS_ELEMENT
					+ " value = \"false\"/>");
			writer.println("  <" + COLOR_ELEMENT
					+ " value = \"white 0 red 1 black 2\"/>");
			writer.println("  <" + LINKDATAFILE_ELEMENT + " value = \""
					+ (new File(this.toDataFile)).getAbsolutePath() + "\"/>");
			writer.println("  <" + MULTILANE_ELEMENT + " value = \"false\"/>");
			writer.println("  <" + ANTIALIASING_ELEMENT
					+ " value = \"false\"/>");

			// final String netType =
			// this.network.getAttr(BasicNetwork.TYPE_ATTRIBUTE);
			final String netType = this.network.getType();
			writer.println("  <" + NETWORKTYPE_ELEMENT + " value = \""
					+ netType + "\"/>");
			if (MATSIM_NETWORK_TYPE.equals(netType)
					|| OPENSTREETMAP_NETWORK_TYPE.equals(netType)) {
				writer.println("  <"
						+ NetvisFromFileRunner.NETWORKFILENAME_ELEMENT
						+ " value = \""
						+ this.networkLoaderConfig.absolutePath(this.networkLoaderConfig
								.get(NETWORKLOADER_ELEMENT,
										AbstractNetworkLoader.FILENAME_ELEMENT))
						+ "\"/>");
			} else if (SUMO_NETWORK_TYPE.equals(netType)) {
				writer.println("  <"
						+ NODEFILE_ELEMENT
						+ " value = \""
						+ this.networkLoaderConfig
								.absolutePath(this.networkLoaderConfig
										.get(NETWORKLOADER_ELEMENT,
												NODEFILE_ELEMENT)) + "\"/>");
				writer.println("  <"
						+ EDGEFILE_ELEMENT
						+ " value = \""
						+ this.networkLoaderConfig
								.absolutePath(this.networkLoaderConfig
										.get(NETWORKLOADER_ELEMENT,
												EDGEFILE_ELEMENT)) + "\"/>");
				final String connectionFile = this.networkLoaderConfig.get(
						NETWORKLOADER_ELEMENT,
						SUMOLoader.CONNECTIONFILE_ELEMENT);
				if (connectionFile != null) {
					writer.println("  <"
							+ CONNECTIONFILE_ELEMENT
							+ " value = \""
							+ this.networkLoaderConfig.get(
									NETWORKLOADER_ELEMENT,
									CONNECTIONFILE_ELEMENT) + "\"/>");
				}
			} else if (!writeUnknownNetworkLoaderConfig(netType, writer,
					this.networkLoaderConfig)) {
				writer.flush();
				writer.close();
				throw new IllegalArgumentException("unknown network type");
			}

			writer.println("</" + NETVISCONFIG_ELEMENT + ">");
			writer.flush();
			writer.close();
		} catch (FileNotFoundException e) {
			throw new RuntimeException(e);
		}
	}

	protected boolean writeUnknownNetworkLoaderConfig(final String networkType,
			final PrintWriter writer, final Config networkLoaderConfig) {
		return false;
	}

	@Override
	public void startOdPair(final BasicNode origin, final BasicNode destination) {
		// this.allLinks = new HashSet<BasicLink>();
	}

	@Override
	// TODO CHANGED
	public void startPath(final List<BasicNode> nodePath,
			final List<BasicLink> linkPath, final Attributes attrs) {
		// final List<BasicLink> linkPath = Router.toLinkRoute(nodePath); //
		// TODO
		/*
		 * (1) bookkeeping
		 */
		// this.allLinks.addAll(linkPath);
		/*
		 * (3) memory management
		 */
		if (this.nextBin >= this.data.getBinCnt()) {
			this.data.resize((int) (1.2 * this.data.getBinCnt()));
		}

		/*
		 * (2) update
		 */
		if (this.nextBin > 0) {
			for (BasicLink link : this.network.getLinks()) {
				this.data
						.put(link, this.nextBin, Math.abs(this.data
								.getBinValue(link, this.nextBin - 1)));
			}
		}
		for (BasicLink link : linkPath) {
			this.data.put(link, this.nextBin,
					-this.data.getBinValue(link, this.nextBin) - 1.0);
		}

		this.nextBin++;

		/*
		 * (4) update
		 */
		// for (BasicLink link : this.allLinks) {
		// this.data.put(link, this.nextBin, 1.0);
		// }
		// for (BasicLink link : linkPath) {
		// this.data.put(link, this.nextBin, 2.0);
		// }
		// this.nextBin++;
	}

	@Override
	public void endPath() {
	}

	@Override
	public void endOdPair() {
	}

	@Override
	public void endPaths() {

		this.data.resize(Math.max(this.nextBin + 1, 1));

		// duplicate very last snapshot
		for (BasicLink link : this.network.getLinks()) {
			this.data.put(link, this.data.getBinCnt() - 1,
					this.data.getBinValue(link, this.data.getBinCnt() - 2));
		}

		// normalize and add path to all but the last snapshot
		for (int bin = 0; bin < this.data.getBinCnt(); bin++) {
			double max = 1; // to avoid division by zero
			for (BasicLink link : this.network.getLinks()) {
				max = Math.max(max, Math.abs(this.data.getBinValue(link, bin)));
			}
			for (BasicLink link : this.network.getLinks()) {
				final double val = this.data.getBinValue(link, bin);
				if ((val >= 0) || (bin == this.data.getBinCnt() - 1)) {
					this.data.put(link, bin, Math.abs(val) / max);
				} else {
					this.data.put(link, bin, 2.0);
				}
			}
		}

		if (this.toDataFile != null) {
			try {
				this.write(this.toDataFile, this.data);
			} catch (IOException e) {
				throw new RuntimeException(e);
			}
		}
	}

	// --------------- IMPLEMENTATION OF DynamicDataXMLFileIO ---------------

	@Override
	protected BasicLink attrValue2key(final String id) {
		return this.network.getLink(id);
	}

	@Override
	protected String key2attrValue(final BasicLink link) {
		return link.getId().toString();
	}
}

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
package floetteroed.bioroute.networkloader;

import static floetteroed.bioroute.BiorouteRunner.NETWORKLOADER_ELEMENT;

import floetteroed.bioroute.NetworkLoader;
import floetteroed.utilities.config.Config;
import floetteroed.utilities.networks.basic.BasicNetwork;
import floetteroed.utilities.networks.basic.BasicNetworkFactory;
import floetteroed.utilities.networks.construction.NetworkContainer;
import floetteroed.utilities.networks.containerloaders.SUMONetworkContainerLoader;

/**
 * Loads a subset of the SUMO network XML format(s).
 * 
 * @author Gunnar Flötteröd
 * 
 */
public class SUMOLoader implements NetworkLoader {

	// -------------------- CONSTANTS --------------------

	public static final String NODEFILE_ELEMENT = "nodefile";

	public static final String EDGEFILE_ELEMENT = "edgefile";

	public static final String CONNECTIONFILE_ELEMENT = "connectionfile";

	// -------------------- MEMBERS --------------------

	private String nodeFile = null;

	private String edgeFile = null;

	private String connectionFile = null;

	// -------------------- CONSTRUCTION --------------------

	public SUMOLoader() {
	}

	// -------------------- IMPLEMENTATION OF NetworkLoader --------------------

	@Override
	public void configure(final Config config) {
		this.nodeFile = config.get(NETWORKLOADER_ELEMENT, NODEFILE_ELEMENT);
		if (this.nodeFile == null) {
			throw new IllegalArgumentException(
					"There is no node file specified. "
							+ "Check if the following configuration entry exists: "
							+ NETWORKLOADER_ELEMENT + "." + NODEFILE_ELEMENT);
		}
		this.nodeFile = config.absolutePath(this.nodeFile);
		this.edgeFile = config.get(NETWORKLOADER_ELEMENT, EDGEFILE_ELEMENT);
		if (this.edgeFile == null) {
			throw new IllegalArgumentException(
					"There is no edge file specified. "
							+ "Check if the following configuration entry exists: "
							+ NETWORKLOADER_ELEMENT + "." + EDGEFILE_ELEMENT);
		}
		this.edgeFile = config.absolutePath(this.edgeFile);
		this.connectionFile = config.get(NETWORKLOADER_ELEMENT,
				CONNECTIONFILE_ELEMENT); // OK to be null
		if (this.connectionFile != null) {
			this.connectionFile = config.absolutePath(this.connectionFile);
		}
	}

	@Override
	public BasicNetwork loadNetwork() {
		// final SUMONetworkLoader loader = new SUMONetworkLoader();
		// return loader.loadNetwork(this.nodeFile, this.edgeFile,
		// this.connectionFile);
		final NetworkContainer container = (new SUMONetworkContainerLoader())
				.loadNetworkContainer(nodeFile, edgeFile, connectionFile);
		return (new BasicNetworkFactory()).newNetwork(container);
	}
}

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

import floetteroed.utilities.networks.basic.BasicNetwork;
import floetteroed.utilities.networks.basic.BasicNetworkFactory;
import floetteroed.utilities.networks.construction.NetworkContainer;
import floetteroed.utilities.networks.containerloaders.MATSimNetworkContainerLoader;

/**
 * Loads a subset of the MATSim network XML file format.
 * 
 * @author Gunnar Flötteröd
 * 
 */
public class MATSimLoader extends AbstractNetworkLoader {

	// -------------------- CONSTRUCTION --------------------

	public MATSimLoader() {
	}

	// ---------- REMAINING IMPLEMENTATION OF NetworkLoader ----------

	@Override
	public BasicNetwork loadNetwork() {		
		final NetworkContainer container = (new MATSimNetworkContainerLoader())
				.load(this.getFileName());
		return (new BasicNetworkFactory()).newNetwork(container);
	}
}

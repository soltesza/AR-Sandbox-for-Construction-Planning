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
package floetteroed.bioroute.utilities;

import floetteroed.bioroute.NetworkLoader;
import floetteroed.bioroute.NetworkPreprocessor;
import floetteroed.utilities.config.Config;
import floetteroed.utilities.config.ConfigurableInstantiator;
import floetteroed.utilities.networks.basic.BasicNetwork;

/**
 * Instantiates and preprocesses a <code>Network</code>.
 * 
 * @author Gunnar Flötteröd
 * 
 */
public class NetworkInstantiator {

	// -------------------- CONSTRUCTION --------------------

	private NetworkInstantiator() {
		// not to be instantiated
	}

	// -------------------- IMPLEMENTATION --------------------

	public static BasicNetwork loadNetwork(final Config config,
			final String... elements) {
		final NetworkLoader networkLoader = (NetworkLoader) ConfigurableInstantiator
				.newConfiguredInstance(config, elements);
		return networkLoader.loadNetwork();
	}

	public static void preprocessNetwork(final BasicNetwork network,
			final Config config, final String... elements) {
		final NetworkPreprocessor networkPreprocessor = (NetworkPreprocessor) ConfigurableInstantiator
				.newConfiguredInstance(config, elements);
		networkPreprocessor.preprocess(network);
	}
}

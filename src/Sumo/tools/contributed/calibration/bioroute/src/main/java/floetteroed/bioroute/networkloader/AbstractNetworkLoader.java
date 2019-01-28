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

/**
 * Abstract implementation of a basic network loader.
 * 
 * @author Gunnar Flötteröd
 * 
 */
public abstract class AbstractNetworkLoader implements NetworkLoader {

	// -------------------- CONSTANTS --------------------

	public static final String FILENAME_ELEMENT = "filename";

	// -------------------- MEMBERS --------------------

	private String fileName = null;

	// -------------------- CONSTRUCTION --------------------

	public AbstractNetworkLoader() {
		// no-argument constructor for reflective instantiation
	}

	// ---------- PARTIAL IMPLEMENTATION OF NetworkLoader ----------

	@Override
	public void configure(final Config config) {
		this.fileName = config.get(NETWORKLOADER_ELEMENT, FILENAME_ELEMENT);
		if (this.fileName == null) {
			throw new IllegalArgumentException(
					"There is not network file specified. "
							+ "Check if the following configuration entry exists: "
							+ NETWORKLOADER_ELEMENT + "." + FILENAME_ELEMENT);
		}
		this.fileName = config.absolutePath(this.fileName);
	}

	@Override
	public abstract BasicNetwork loadNetwork();

	// -------------------- GETTERS --------------------

	public String getFileName() {
		return this.fileName;
	}
}

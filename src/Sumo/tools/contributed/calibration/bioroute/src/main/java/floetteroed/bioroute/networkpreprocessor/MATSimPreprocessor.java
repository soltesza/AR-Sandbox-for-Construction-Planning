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
package floetteroed.bioroute.networkpreprocessor;

import floetteroed.bioroute.NetworkPreprocessor;
import floetteroed.utilities.config.Config;
import floetteroed.utilities.networks.basic.BasicLink;
import floetteroed.utilities.networks.basic.BasicNetwork;

/**
 * Preprocessor for MATSim networks. Infers the free flow travel time from link
 * length and free flow speed.
 * 
 * @author Gunnar Flötteröd
 * 
 */
public class MATSimPreprocessor implements NetworkPreprocessor {

	// -------------------- CONSTANTS --------------------

	/**
	 * Required link attribute for the preprocessing; also available afterwards.
	 */
	public static final String FREESPEED_ATTRIBUTE = "freespeed";

	/**
	 * Required link attribute for the preprocessing; also available afterwards.
	 */
	public static final String LENGTH_ATTRIBUTE = "length";

	/**
	 * Available link attribute after the preprocessing.
	 */
	public static final String TRAVELTIME_ATTRIBUTE = "traveltime";

	// -------------------- CONSTRUCTION --------------------

	public MATSimPreprocessor() {
	}

	// -------------------- CONSTANTS --------------------

	@Override
	public void configure(final Config config) {
	}

	@Override
	public void preprocess(final BasicNetwork network) {
		for (BasicLink link : network.getLinks()) {
			final double length_m = Double.parseDouble(link
					.getAttr(LENGTH_ATTRIBUTE));
			final double vMax_m_s = Double.parseDouble(link
					.getAttr(FREESPEED_ATTRIBUTE));
			final double tt_s = length_m / vMax_m_s;
			link.setAttr(TRAVELTIME_ATTRIBUTE, Double.toString(tt_s));
		}
	}
}

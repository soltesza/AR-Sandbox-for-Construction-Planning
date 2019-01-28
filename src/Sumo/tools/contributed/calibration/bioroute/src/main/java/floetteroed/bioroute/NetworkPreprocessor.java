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

import floetteroed.utilities.config.Configurable;
import floetteroed.utilities.networks.basic.BasicNetwork;

/**
 * <strong>BIOROUTE</strong> interface. Defines the network preprocessor.
 * 
 * @author Gunnar Flötteröd
 * 
 */
public interface NetworkPreprocessor extends Configurable {

	/**
	 * Preprocesses the network.
	 * 
	 * @param network
	 *            the network; possibly modified by a call to this function
	 */
	public void preprocess(final BasicNetwork network);

}

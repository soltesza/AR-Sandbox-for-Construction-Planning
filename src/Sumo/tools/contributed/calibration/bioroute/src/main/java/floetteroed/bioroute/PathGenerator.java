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
import floetteroed.utilities.networks.basic.BasicNode;

/**
 * <strong>BIOROUTE</strong> interface. Defines the path generation algorithm.
 * 
 * @author Gunnar Flötteröd
 * 
 */
public interface PathGenerator extends Configurable {

	/**
	 * Sets the network. It is called after <code>configure</code> (inherited
	 * from <code>Configurable</code>) and before <code>setPathWriter</code>.
	 * 
	 * @param network
	 *            the network
	 */
	public void setNetwork(final BasicNetwork network);

	/**
	 * Sets the path writer. It is called after <code>setNetwork</code> and
	 * (once) before all calls to <code>run</code>.
	 * <p>
	 * The implementing class is expected to call
	 * {@link floetteroed.bioroute.PathWriter#writePath(java.util.List, java.util.Map)} once
	 * for every generated path. All other functions of <code>PathWriter</code>
	 * are called by <code>BiorouteRunner</code>.
	 * 
	 * @param writer
	 *            the path writer
	 */
	public void setPathWriter(final PathWriter writer);

	/**
	 * TODO changed this back to node-based representation
	 * 
	 * Generates paths for the indicated OD pair. Is called once for every
	 * origin/destination pair in the XML configuration file.
	 * 
	 * @param origin
	 *            the origin
	 * @param destination
	 *            the destination
	 */
	public void run(final BasicNode origin, final BasicNode destination);

}

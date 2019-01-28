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

import java.util.List;
import java.util.Map;

import floetteroed.utilities.config.Configurable;
import floetteroed.utilities.networks.basic.BasicLink;


/**
 * <strong>BIOROUTE</strong> interface. Defines the path writer.
 * 
 * @author Gunnar Flötteröd
 * 
 */
public interface PathWriter extends Configurable {

	/**
	 * Opens the path writer. Called once before the writing is started.
	 */
	public void open();

	/**
	 * TODO origin in the form of nodes or links or ???
	 * 
	 * Indicates that the following paths apply to the OD pair
	 * <code>from/to</code>. Called once per OD pair, before all respective
	 * calls to <code>writePath</code>.
	 * 
	 * @param from
	 *            the origin link
	 * @param to
	 *            the destination link
	 * 
	 */
	public void startOdPair(final String from, final String to);

	/**
	 * Writes <code>path</code> to file.
	 * 
	 * @param path
	 *            the path, represented by a list of link IDs
	 * @param attrs
	 *            additional attribute/value pairs that are to be written
	 *            together with this path
	 */
	public void writePath(final List<BasicLink> path,
			final Map<String, String> attrs);

	/**
	 * Indicates that all paths for the current OD pair have been written.
	 * Called once pair OD pair, after all calls to <code>writePath</code>.
	 */
	public void endOdPair();

	/**
	 * Closes the path writer. Called once at the end of all writing.
	 */
	public void close();
}

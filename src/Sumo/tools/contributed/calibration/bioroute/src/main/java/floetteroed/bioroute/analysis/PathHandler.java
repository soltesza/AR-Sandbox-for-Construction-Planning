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

import java.util.List;

import org.xml.sax.Attributes;

import floetteroed.utilities.networks.basic.BasicLink;
import floetteroed.utilities.networks.basic.BasicNode;


/**
 * A handler for the <code>PathXMLParser</code>.
 * 
 * @author Gunnar Flötteröd
 * 
 */
public interface PathHandler {

	public void startPaths(final Attributes attrs);

	// TODO CHANGED
	public void startOdPair(final BasicNode origin, final BasicNode destination);

	// TODO CHANGED
	public void startPath(final List<BasicNode> nodePath,
			final List<BasicLink> linkPath, final Attributes attrs);

	public void endPath();

	public void endOdPair();

	public void endPaths();

}

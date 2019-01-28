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
package floetteroed.bioroute.pathwriter;

import static floetteroed.bioroute.BiorouteRunner.PATHWRITER_CONFIG_ELEMENT;

import java.io.FileNotFoundException;
import java.io.PrintWriter;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;

import floetteroed.bioroute.PathWriter;
import floetteroed.utilities.config.Config;
import floetteroed.utilities.math.MathHelpers;
import floetteroed.utilities.networks.basic.BasicLink;

/**
 * Writes paths in an XML file.
 * 
 * @author Gunnar Flötteröd
 * 
 */
public class PathXMLWriter implements PathWriter {

	// -------------------- CONSTANTS --------------------

	// DEFAULT VALUES

	public static final Boolean DEFAULT_SKIPDUPLICATES = false;

	// CONFIGURATION XML SPECIFICATION

	public static final String FILENAME_ELEMENT = "filename";

	public static final String SKIPDUPLICATES_ELEMENT = "skipduplicates";

	// PATH FILE XML SPECIFICATION

	public static final String PATHS_ELEMENT = "paths";

	public static final String ODPAIR_ELEMENT = "odpair";

	public static final String FROM_ATTRIBUTE = "from";

	public static final String TO_ATTRIBUTE = "to";

	public static final String PATH_ELEMENT = "path";

	public static final String NODES_ATTRIBUTE = "nodes";

	public static final String LINKS_ATTRIBUTE = "links";

	// -------------------- MEMBERS --------------------

	// CONFIGURATION

	private String fileName = null;

	private Boolean skipDuplicates = null;

	// RUNTIME

	private PrintWriter writer = null;

	private final Set<List<BasicLink>> pathsSoFar = new HashSet<List<BasicLink>>();

	// -------------------- CONSTUCTION --------------------

	public PathXMLWriter() {
		// no-argument constructor for reflective instantiation
	}

	// -------------------- IMPLEMENTATION OF PathWriter --------------------

	@Override
	public void configure(final Config config) {

		this.fileName = config.get(PATHWRITER_CONFIG_ELEMENT, FILENAME_ELEMENT);
		if (fileName == null) {
			throw new IllegalArgumentException(FILENAME_ELEMENT
					+ " is not specified");
		}
		this.fileName = config.absolutePath(this.fileName);

		this.skipDuplicates = MathHelpers.parseBoolean(config.get(
				PATHWRITER_CONFIG_ELEMENT, SKIPDUPLICATES_ELEMENT));
		if (this.skipDuplicates == null) {
			this.skipDuplicates = DEFAULT_SKIPDUPLICATES;
		}
	}

	@Override
	public void open() {
		try {
			this.writer = new PrintWriter(this.fileName);
		} catch (FileNotFoundException e) {
			throw new RuntimeException(e);
		}
		this.writer.println("<" + PATHS_ELEMENT + ">");
	}

	@Override
	public void startOdPair(final String from, final String to) {
		this.pathsSoFar.clear();
		this.writer.println("  <" + ODPAIR_ELEMENT + " " + FROM_ATTRIBUTE
				+ "=\"" + from + "\" " + TO_ATTRIBUTE + "=\"" + to + "\">");
	}

	// private String nodesAndLinksToString(final List<BasicLink> path) {
	// final StringBuffer nodes = new StringBuffer(NODES_ATTRIBUTE + "=\"");
	// final StringBuffer links = new StringBuffer(LINKS_ATTRIBUTE + "=\"");
	// for (BasicLink link : path) {
	// nodes.append(link.getFromNode().getId());
	// nodes.append(" ");
	// links.append(link.getId());
	// links.append(" ");
	// }
	// nodes.append(path.get(path.size() - 1).getToNode().getId());
	// nodes.append(" ");
	// return nodes.toString() + " " + links.toString();
	// }

	// private List<String> pathNodeIDs(final List<BasicLink> path) {
	// final List<String> result = new ArrayList<String>(path.size() + 1);
	// result.add(path.get(0).getFromNode().getId());
	// for (BasicLink link : path) {
	// result.add(link.getToNode().getId());
	// }
	// return result;
	// }

	@Override
	public void writePath(final List<BasicLink> linkPath,
			final Map<String, String> attrs) {

		// TODO NEW
		// final List<String> path = this.pathNodeIDs(linkPath);

		if (this.skipDuplicates && this.pathsSoFar.contains(linkPath)) {
			return;
		}
		this.pathsSoFar.add(linkPath);

		this.writer.print("    <" + PATH_ELEMENT + " ");

		final StringBuffer nodes = new StringBuffer(NODES_ATTRIBUTE + "=\"");
		final StringBuffer links = new StringBuffer(LINKS_ATTRIBUTE + "=\"");
		for (int i = 0; i < linkPath.size() - 1; i++) {
			final BasicLink link = linkPath.get(i);
			nodes.append(link.getFromNode().getId());
			nodes.append(" ");
			links.append(link.getId());
			links.append(" ");
		}
		final BasicLink link = linkPath.get(linkPath.size() - 1);
		nodes.append(link.getFromNode().getId());
		nodes.append(" ");
		nodes.append(link.getToNode().getId());
		nodes.append("\" ");
		links.append(link.getId());
		links.append("\" ");
		this.writer.print(nodes);
		this.writer.print(links);

		// this.writer.print(LINKS_ATTRIBUTE + "=\"");
		// for (int i = 0; i < path.size() - 1; i++) {
		// this.writer.print(path.get(i) + " ");
		// }
		// this.writer.print(path.get(path.size() - 1) + "\"");

		if (attrs != null) {
			for (Map.Entry<String, String> entry : attrs.entrySet()) {
				this.writer.print(entry.getKey() + "=\"" + entry.getValue()
						+ "\"");
			}
		}
		this.writer.println("/>");
	}

	@Override
	public void endOdPair() {
		this.writer.println("  </" + ODPAIR_ELEMENT + ">");
		this.writer.flush(); // so one can follow the progress in the file
	}

	@Override
	public void close() {
		this.writer.println("</" + PATHS_ELEMENT + ">");
		this.writer.flush();
		this.writer.close();
	}
}

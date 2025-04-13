/**
 * Represents a person.
 * @typedef {Object} NodeType
 * @property {string} Id Node Id (path).
 * @property {string} Name Node name.
 * @property {string} Type Node type: "File"|"Folder".
 * @property {Array<NodeType> | null} Children List of children nodes.
 */

/**
 * Checks if element exist.
 * @param {HTMLElement} element Html element.
 * @param {string} name Element name.
 * @returns True if element exist.
 */
const isElementExist = (element, name) => {
  if (!element) {
    console.error(`'${name}' element not found!`);
    return false;
  }
  return true;
}

/**
 * Calculates the relative path between two node paths.
 * @param {string} nodePathFrom Source node path.
 * @param {string} nodePathTo Target node path.
 * @returns Path from the first node to the second.
 */
const getRelativePath = (nodePathFrom, nodePathTo) => {
  const pathPartsFrom = nodePathFrom.split('/').slice(0, -1);
  const pathPartsTo = nodePathTo.split('/');

  // Find the first index where the paths differ
  let commonBaseIndex  = 0;
  while (commonBaseIndex < pathPartsFrom.length
    && commonBaseIndex < pathPartsTo.length
    && pathPartsFrom[commonBaseIndex] === pathPartsTo[commonBaseIndex])
  {
    commonBaseIndex++;
  }

  // Navigate up from the `from` path to the common base directory
  const stepsUp = "../".repeat(pathPartsFrom.length - commonBaseIndex);

  // Navigate down to the `to` path from the common base
  const stepsDown = pathPartsTo.slice(commonBaseIndex).join('/');

  return `${stepsUp}${stepsDown}`;
}

/**
 * Generates HTML from JSON data with a tree structure.
 * @param {Array<NodeType>} nodeList List of nodes in root folder.
 * @param {string} currentNodeId Current node Id (path).
 * @returns {string} HTML tree structure.
 */
const generateHtmlTree = (nodeList, currentNodeId) => {
  if (!nodeList || nodeList.length === 0) {
    return "";
  }

  let result = "";
  for (const node of nodeList) {
    if (node.Type === "Folder" && node.Children && node.Children.length !== 0) {
      result += `<details id="${node.Id}/"><summary>${node.Name}</summary><div class="tree-group">`;
      result += generateHtmlTree(node.Children, currentNodeId);
      result += "</div></details>";
    } else if (node.Type === "File") {
      const nodeLink = currentNodeId
        ? getRelativePath(currentNodeId, node.Id)
        : node.Id;

      result += `<a id="${node.Id}" href="${nodeLink}" class="tree-item">${node.Name}</a>`;
    }
  }
  return result;
}

/**
 * Opens all subfolders in node's path and highlight the currently open node.
 * @param {HTMLElement} treeView Tree view element.
 * @param {string} currentNodeId Current node Id (path).
 */
const openFolderNodes = (treeView, currentNodeId) => {
  if (currentNodeId) {
    const currentFileNode = document.getElementById(currentNodeId);
    if (currentFileNode) {
      currentFileNode.classList.add("tree-item-current");
      const indexes = [...currentNodeId.matchAll(new RegExp("/", "gi"))].map(a => a.index);
      indexes.forEach(index => {
        const folderId = currentNodeId.slice(0, index + 1);
        let folderNode = document.getElementById(folderId);
        if (folderNode) {
          folderNode.open = true;
        }
      });

      return;
    }
  }

  // opens all folders if there is no nodeId (index page)
  const details = Array.from(treeView.querySelectorAll("details"))
  details.forEach(element => {
    element.open = true;
  });
}

/**
 * Sets the Tree View.
 * @param {Array<NodeType>} nodeList List of nodes in root folder.
 */
const setUpTree = (nodeList) => {
  const treeView = document.getElementById("nav-tree");
  if (!isElementExist(treeView, "nav-tree")) {
    return;
  }

  if (nodeList.length === 0) {
    console.log("nodeList is empty");
    return;
  }

  const currentNodeId = treeView.dataset.node;
  treeView.innerHTML = generateHtmlTree(nodeList, currentNodeId);
  openFolderNodes(treeView, currentNodeId);
}

/**
 * Adds HTML headings (h1, h2) to the content table on the right for quick navigation.
 */
const setUpContentTable = () => {
  const blog = document.getElementById("markdown");
  const tableOfContents = document.getElementById("content-table");

  if (!isElementExist(blog, "markdown") ||
    !isElementExist(tableOfContents, "content-table")) {
    return;
  }

  const headers = blog.querySelectorAll("h1, h2");
  for (let i = 0; i < headers.length; i++) {
    const li = document.createElement("li");
    const a = document.createElement("a");
    a.innerHTML = headers[i].textContent;
    a.href = `#${headers[i].id}`;
    li.appendChild(a);
    tableOfContents.appendChild(li);
  }
}

/**
 * Shows/hides the sidebar.
 */
const setUpSidebar = () => {
  const btn = document.getElementById("btn-menu");
  const sidebar = document.getElementById("sidebar");

  if (!isElementExist(btn, "btn-menu") ||
    !isElementExist(sidebar, "sidebar")) {
    return;
  }

  btn.onclick = () => {
    if (sidebar.style.display === "block") {
      sidebar.style.display = "none";
    } else {
      sidebar.style.display = "block";
    }
  };
}

/**
 * Entry point function.
 * @param {Array<NodeType>} nodeList List of nodes in root folder.
 */
const main = (nodeList) => {
  setUpTree(nodeList);
  setUpContentTable();
  setUpSidebar();
}

// "nodeList" is taken from "tree.js"
main(nodeList);
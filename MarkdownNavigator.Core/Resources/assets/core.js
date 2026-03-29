/**
 * Represents a tree node.
 * @typedef {Object} NodeType
 * @property {string} Path Node path (relative HTML path).
 * @property {string} Name Node name.
 * @property {string} Type Node type: "File"|"Folder".
 * @property {Array<NodeType> | null} Children List of children nodes.
 */

/**
 * Checks that an HTML element exists.
 * Logs an error and returns false when it is missing.
 * @param {HTMLElement | null} element
 * @param {string} name
 * @returns {boolean}
 */
const isElementExist = (element, name) => {
    if (!element) {
        console.error(`'${name}' element not found!`);
        return false;
    }

    return true;
};

/**
 * Calculates the relative path between two node paths.
 * @param {string} nodePathFrom Source node path.
 * @param {string} nodePathTo Target node path.
 * @returns {string}
 */
const getRelativePath = (nodePathFrom, nodePathTo) => {
    const pathPartsFrom = nodePathFrom.split("/").slice(0, -1);
    const pathPartsTo = nodePathTo.split("/");

    let commonBaseIndex = 0;
    while (
        commonBaseIndex < pathPartsFrom.length &&
        commonBaseIndex < pathPartsTo.length &&
        pathPartsFrom[commonBaseIndex] === pathPartsTo[commonBaseIndex]
    ) {
        commonBaseIndex++;
    }

    const stepsUp = "../".repeat(pathPartsFrom.length - commonBaseIndex);
    const stepsDown = pathPartsTo.slice(commonBaseIndex).join("/");

    return `${stepsUp}${stepsDown}`;
};

/**
 * Returns a link to the target node.
 * If currentNodePath is empty, returns nodePath as is.
 * @param {string} currentNodePath
 * @param {string} nodePath
 * @returns {string}
 */
const getNodeLink = (currentNodePath, nodePath) => {
    return currentNodePath
        ? getRelativePath(currentNodePath, nodePath)
        : nodePath;
};

/**
 * Returns the closest folder node for the current element.
 * If the current element itself is a folder, returns it directly.
 * @param {Element | null} node
 * @returns {Element | null}
 */
const getFolderNode = (node) => {
    if (!node) {
        return null;
    }

    return node.classList.contains("tree-folder")
        ? node
        : node.closest(".tree-folder");
};

/**
 * Generates HTML from JSON data with a tree structure.
 * @param {Array<NodeType>} nodeList List of nodes in root folder.
 * @param {string} currentNodePath Current node path.
 * @returns {string}
 */
const generateHtmlTree = (nodeList, currentNodePath) => {
    if (!nodeList?.length) {
        return "";
    }

    return nodeList
        .map((node) => {
            const link = getNodeLink(currentNodePath, node.Path);

            if (node.Type === "Folder" && node.Children?.length) {
                return `<div class="tree-folder" id="${node.Path}">
    <div class="tree-folder-header">
        <span class="tree-folder-toggle"></span>
        <a href="${link}" class="tree-folder-link">${node.Name}</a>
    </div>
    <div class="tree-group" hidden>
        ${generateHtmlTree(node.Children, currentNodePath)}
    </div>
</div>`;
            }

            if (node.Type === "File") {
                return `<a id="${node.Path}" href="${link}" class="tree-item">${node.Name}</a>`;
            }

            return "";
        })
        .join("");
};

/**
 * Expands a folder node.
 * @param {Element} folderNode
 */
const expandFolder = (folderNode) => {
    const toggle = folderNode.querySelector(".tree-folder-toggle");
    const group = folderNode.querySelector(".tree-group");

    if (!toggle || !group) {
        return;
    }

    toggle.classList.add("expanded");
    group.hidden = false;
};

/**
 * Expands all folder nodes inside the provided tree view.
 * @param {HTMLElement} treeView
 */
const expandAllFolders = (treeView) => {
    treeView.querySelectorAll(".tree-folder").forEach(expandFolder);
};

/**
 * Highlights the current node in the tree.
 * If the current node is a folder wrapper, highlights its link.
 * @param {Element} currentNode
 */
const highlightCurrentNode = (currentNode) => {
    if (currentNode.classList.contains("tree-folder")) {
        const link = currentNode.querySelector(".tree-folder-link");
        if (link) {
            link.classList.add("tree-item-current");
        }

        return;
    }

    currentNode.classList.add("tree-item-current");
};

/**
 * Expands all parent folders for the current node.
 * @param {Element} currentNode
 */
const expandParentFolders = (currentNode) => {
    let folderNode = getFolderNode(currentNode);

    while (folderNode) {
        expandFolder(folderNode);
        folderNode = folderNode.parentElement?.closest(".tree-folder");
    }
};

/**
 * Finds the top-level folder branch that contains the current node.
 * @param {HTMLElement} treeView
 * @param {Element} currentNode
 * @returns {Element | null}
 */
const getTopLevelFolderNode = (treeView, currentNode) => {
    let folderNode = getFolderNode(currentNode);
    let topLevelFolder = folderNode;

    while (folderNode) {
        const parentFolder = folderNode.parentElement?.closest(".tree-folder");
        if (!parentFolder || !treeView.contains(parentFolder)) {
            break;
        }

        topLevelFolder = parentFolder;
        folderNode = parentFolder;
    }

    return topLevelFolder;
};

/**
 * Expands the full branch starting from the provided folder:
 * the folder itself and all nested folders inside it.
 * @param {Element | null} folderNode
 */
const expandFolderBranch = (folderNode) => {
    if (!folderNode) {
        return;
    }

    expandFolder(folderNode);
    folderNode.querySelectorAll(".tree-folder").forEach(expandFolder);
};

/**
 * Opens tree folders based on the current node path.
 * @param {HTMLElement} treeView
 * @param {string} currentNodePath
 */
const openFolderNodes = (treeView, currentNodePath) => {
    if (!currentNodePath) {
        expandAllFolders(treeView);
        return;
    }

    const currentNode = document.getElementById(currentNodePath);
    if (!currentNode) {
        expandAllFolders(treeView);
        return;
    }

    highlightCurrentNode(currentNode);
    expandParentFolders(currentNode);

    const topLevelFolder = getTopLevelFolderNode(treeView, currentNode);
    expandFolderBranch(topLevelFolder);
};

/**
 * Sets up folder toggle click handler once for the tree view.
 * Prevents duplicate handlers when tabs are re-rendered.
 * @param {HTMLElement} treeView
 */
const setUpFolderToggles = (treeView) => {
    if (treeView.dataset.folderToggleBound === "true") {
        return;
    }

    treeView.addEventListener("click", (e) => {
        const toggle = e.target.closest(".tree-folder-toggle");
        if (!toggle) {
            return;
        }

        e.stopPropagation();

        const folder = toggle.closest(".tree-folder");
        const group = folder?.querySelector(".tree-group");
        if (!group) {
            return;
        }

        const isExpanded = !group.hidden;
        group.hidden = isExpanded;
        toggle.classList.toggle("expanded", !isExpanded);
    });

    treeView.dataset.folderToggleBound = "true";
};

/**
 * Adds HTML headings (h1, h2) to the content table on the right for quick navigation.
 */
const setUpContentTable = () => {
    const blog = document.getElementById("markdown");
    const tableOfContents = document.getElementById("content-table");

    if (
        !isElementExist(blog, "markdown") ||
        !isElementExist(tableOfContents, "content-table")
    ) {
        return;
    }

    const headers = blog.querySelectorAll("h1, h2");
    const fragment = document.createDocumentFragment();

    for (const header of headers) {
        const li = document.createElement("li");
        const a = document.createElement("a");

        a.textContent = header.textContent;
        a.href = `#${header.id}`;

        li.appendChild(a);
        fragment.appendChild(li);
    }

    tableOfContents.appendChild(fragment);
};

/**
 * Detects the active tab based on the current node path.
 * @param {string} nodePath
 * @returns {'wiki'|'tasks'|'qa'}
 */
const detectTab = (nodePath) => {
    if (nodePath.startsWith("_tasks/")) {
        return "tasks";
    }

    if (nodePath.startsWith("_qa/")) {
        return "qa";
    }

    return "wiki";
};

/**
 * Shared renderer for flat sidebar lists such as tasks and Q&A.
 * @param {Array} items
 * @param {string} currentNodePath
 * @param {object} options
 * @param {string} options.emptyText
 * @param {string} options.basePath
 * @param {(item: any) => string} options.getText
 * @param {(item: any) => string} options.getExtraClass
 * @returns {string}
 */
const buildListTree = (items, currentNodePath, options) => {
    const {
        emptyText,
        basePath,
        getText,
        getExtraClass,
    } = options;

    if (!items?.length) {
        return `<p>${emptyText}</p>`;
    }

    return items
        .map((item) => {
            const fullPath = `${basePath}${item.path}`;
            const link = getNodeLink(currentNodePath, fullPath);
            const currentClass = currentNodePath === fullPath
                ? " tree-item-current"
                : "";
            const extraClass = getExtraClass(item);

            return `<a href="${link}" class="tree-item${currentClass}${extraClass}">${getText(item)}</a>`;
        })
        .join("");
};

/**
 * Generates HTML for task list in the sidebar.
 * @param {Array} taskList
 * @param {string} currentNodePath
 * @returns {string}
 */
const generateTaskTree = (taskList, currentNodePath) => {
    return buildListTree(taskList, currentNodePath, {
        emptyText: "No tasks",
        basePath: "_tasks/",
        getText: (task) => task.name,
        getExtraClass: (task) =>
            task.status ? ` task-${task.status.toLowerCase()}` : "",
    });
};

/**
 * Generates HTML for Q&A list in the sidebar.
 * @param {Array} qaList
 * @param {string} currentNodePath
 * @returns {string}
 */
const generateQaTree = (qaList, currentNodePath) => {
    return buildListTree(qaList, currentNodePath, {
        emptyText: "No Q&amp;A items",
        basePath: "_qa/",
        getText: (item) => item.question,
        getExtraClass: (item) => ` qa-${item.popularity}`,
    });
};

/**
 * Builds a cache key for a tab.
 * Cache depends on:
 * - tab name
 * - current node path
 * The current implementation assumes rootNode / tasks / qa do not change at runtime.
 * If they can change, add a data version suffix here.
 * @param {'wiki'|'tasks'|'qa'} tabName
 * @param {string} currentNodePath
 * @returns {string}
 */
const getTabCacheKey = (tabName, currentNodePath) => {
    return `${tabName}::${currentNodePath}`;
};

/**
 * Sets up tab switching for sidebar navigation.
 * Mobile behavior:
 * - click active tab -> hide sidebar
 * - click inactive tab -> show sidebar and switch tab
 * @param {NodeType} rootNode
 * @param {string} currentNodePath
 */
const setUpTabs = (rootNode, currentNodePath) => {
    const sidebar = document.getElementById("sidebar");
    const navTree = document.getElementById("nav-tree");
    const tabs = document.querySelectorAll(".header-tab");
    const isMobile = () => window.innerWidth <= 768;

    if (!isElementExist(sidebar, "sidebar") || !isElementExist(navTree, "nav-tree")
    ) {
        return;
    }

    setUpFolderToggles(navTree);

    let activeTab = detectTab(currentNodePath);

    /**
     * Simple in-memory cache for tab HTML.
     * Key example: "wiki::_tasks/task1.html"
     * Value: generated innerHTML string
     */
    const tabHtmlCache = new Map();

    /**
     * Returns cached HTML or builds it on first request.
     * @param {'wiki'|'tasks'|'qa'} tabName
     * @returns {string}
     */
    const getTabHtml = (tabName) => {
        const cacheKey = getTabCacheKey(tabName, currentNodePath);
        if (tabHtmlCache.has(cacheKey)) {
            return tabHtmlCache.get(cacheKey);
        }

        let html = "";

        switch (tabName) {
            case "wiki":
                if (rootNode?.Children) {
                    html = generateHtmlTree(rootNode.Children, currentNodePath);
                }
                break;

            case "tasks":
                html = generateTaskTree(
                    typeof tasks !== "undefined" ? tasks : [],
                    currentNodePath
                );
                break;

            case "qa":
                html = generateQaTree(
                    typeof qa !== "undefined" ? qa : [],
                    currentNodePath
                );
                break;
        }

        tabHtmlCache.set(cacheKey, html);
        return html;
    };

    /**
     * Renders the selected tab.
     * Uses cached HTML when possible.
     * @param {'wiki'|'tasks'|'qa'} tabName
     */
    const renderTab = (tabName) => {
        navTree.innerHTML = getTabHtml(tabName);

        // Wiki tab needs post-render DOM actions.
        if (tabName === "wiki") {
            openFolderNodes(navTree, currentNodePath);
        }

        tabs.forEach((tab) => {
            tab.classList.toggle("header-tab--active", tab.dataset.tab === tabName);
        });

        activeTab = tabName;
    };

    tabs.forEach((tab) => {
        tab.addEventListener("click", (e) => {
            e.stopPropagation();

            const clickedTab = tab.dataset.tab;

            if (isMobile()) {
                if (clickedTab === activeTab) {
                    sidebar.classList.toggle("sidebar--open");
                    return;
                }

                sidebar.classList.add("sidebar--open");
            }

            if (clickedTab !== activeTab) {
                renderTab(clickedTab);
            }
        });
    });

    renderTab(activeTab);
};

/**
 * Entry point function.
 * @param {NodeType} rootNode
 */
const main = (rootNode) => {
    const navTree = document.getElementById("nav-tree");
    const currentNodePath = navTree ? navTree.dataset.node : "";

    setUpTabs(rootNode, currentNodePath);
    setUpContentTable();
};

// "rootNode", "tasks", "qa" are loaded from "data.js"
main(rootNode);
// applicationUrl from launchSettings.json
const url = "https://localhost:7156"

/**
 * Represents a person.
 * @typedef {Object} NodeType
 * @property {string} Id Node Id.
 * @property {string} Name Node name.
 * @property {string} Type Node type: "File"|"Folder".
 * @property {string | null} Link Link to file local file location for "File" node.
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
 * Generates HTML from JSON data with a tree structure.
 * @param {Array<NodeType>} nodeList List of nodes in root folder.
 * @returns {string} HTML tree structure.
 */
const generateHtmlTree = (nodeList) => {
  if (!nodeList || nodeList.length === 0) {
    return "";
  }

  let result = "";
  for (const node of nodeList) {
    if (node.Type === "Folder" && node.Children && node.Children.length !== 0) {
      result += `<details id="${node.Id}__"><summary>${node.Name}</summary><div class="tree-view-group">`;
      result += generateHtmlTree(node.Children);
      result += '</div></details>';
    } else if (node.Type === "File") {
      result += `<a id="${node.Id}" href="${node.Link}" class="tree-view-item">${node.Name}</a>`;
    }
  }
  return result;
}

/**
 * Sets the Tree View to highlight the currently open file and open all subfolders in that file's path.
 * @param {Array<NodeType>} nodeList List of nodes in root folder.
 */
const setUpTree = (nodeList) => {
  const treeView = document.getElementById("tree-view");
  if (!isElementExist(treeView, "tree-view")) {
    return;
  }

  if (nodeList.length === 0) {
    console.log("nodeList is empty");
    return;
  }

  treeView.innerHTML = generateHtmlTree(nodeList);
  const file = treeView.dataset.file;
  const currentFile = document.getElementById(file);
  if (currentFile) {
    currentFile.classList.add("tree-view-item-current");
    const indexes = [...file.matchAll(new RegExp("__", "gi"))].map(a => a.index);
    indexes.forEach(index => {
      const folderId = file.slice(0, index + 2);
      let folderElement = document.getElementById(folderId);
      if (folderElement) {
        folderElement.open = true;
      }
    });
  } else {
    const details = Array.from(treeView.querySelectorAll("details"))
    details.forEach(element => {
      element.open = true;
    });
  }
}

/**
 * Adds HTML headings (h1, h2) to the content table on the right for quick navigation.
 */
const setUpContentTable = () => {
  const blog = document.getElementById("blog");
  const tableOfContents = document.getElementById("content-table");

  if (!isElementExist(blog, "blog") ||
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
 * Sets the current slide.
 */
const setSlides = (slides, currentSlide) => {
  slides.forEach((slide, indx) => {
    slide.style.transform = `translateX(${(indx - currentSlide) * 100}%)`;
  });
}

/**
 * Sets slide switching.
 */
const setSlider = () => {
  document.querySelectorAll(".slider").forEach((slider) => {
    const slides = slider.querySelectorAll(".slide");
    const nextSlide = slider.querySelector(".button-slider--next");
    const prevSlide = slider.querySelector(".button-slider--prev");
    const maxSlideIndex = slides.length - 1;
    let currentSlideIndex = 0;
    if (nextSlide) {
      nextSlide.onclick = () => {
        currentSlideIndex = (currentSlideIndex === maxSlideIndex) ? 0 : currentSlideIndex + 1;
        setSlides(slides, currentSlideIndex);
      };
    }
    if (prevSlide) {
      prevSlide.onclick = () => {
        currentSlideIndex = (currentSlideIndex === 0) ? maxSlideIndex : currentSlideIndex - 1;
        setSlides(slides, currentSlideIndex);
      };
    }
    setSlides(slides, 0);
  });
}


/**
 * Specifies the logic for editing the markdown file.
 */
const setUpModalButtons = () => {
  const treeView = document.getElementById("tree-view");
  const modal = document.getElementById("modal");
  const textarea = document.getElementById("textarea");
  const btnGet = document.getElementById("btn-markdown-get");
  const btnPost = document.getElementById("btn-markdown-post");
  const btnClose = document.getElementById("btn-modal-close");

  if (!isElementExist(treeView, "tree-view") ||
    !isElementExist(btnGet, "modal") ||
    !isElementExist(btnPost, "btn-markdown-get") ||
    !isElementExist(btnPost, "btn-markdown-post") ||
    !isElementExist(btnClose, "btn-modal-close") ||
    !isElementExist(textarea, "textarea") ||
    !isElementExist(modal, "modal")) {
    return;
  }

  const path = treeView.dataset.path;
  btnGet.onclick = async () => {
    if (path) {
      const response = await fetch(`${url}/markdown?path=${path}`);
      if (response.ok) {
        const text = await response.text();
        textarea.value = text;
      }
      modal.style.display = "block";
    } else {
      console.log("markdown path not found")
    }
  }

  btnPost.onclick = async () => {
    const response = await fetch(`${url}/markdown`,
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          Path: path,
          Text: textarea.value
        })
      }
    );
    if (response.ok) {
      window.location.reload();
    }
  }

  btnClose.onclick = () => {
    modal.style.display = "none";
  }
}

/**
 * Entry point function.
 * @param {Array<NodeType>} nodeList List of nodes in root folder.
 */
const main = (nodeList) => {
  setUpTree(nodeList);
  setUpModalButtons();
  setUpContentTable();
  setUpSidebar();
  setSlider();
}

// "nodeList" is taken from "treeData.js"
main(nodeList);
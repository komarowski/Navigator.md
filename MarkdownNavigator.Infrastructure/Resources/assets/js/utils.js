const addOnClickCopyEvent = (button, element) => {
	button.onclick = () => {
		navigator.clipboard.writeText(element.innerText);
	};
}

const applyCodeCopy = () => {
	const codeElements = document.querySelectorAll("code:not(pre code)");
	codeElements.forEach(code => {
		const button = document.createElement("button");
		button.classList.add("button-copy");
		code.insertAdjacentElement('afterEnd', button);
		addOnClickCopyEvent(button, code);
	});

	const preElements = document.querySelectorAll("pre");
	preElements.forEach(pre => {
		const code = pre.querySelector("code");
		if (code) {
			const button = document.createElement("button");
			button.classList.add("button-copy");
			const buttonDiv = document.createElement("div");
			buttonDiv.classList.add("button-copy-container");
			buttonDiv.appendChild(button);
			pre.insertAdjacentElement('beforeBegin', buttonDiv);
			addOnClickCopyEvent(button, code);
		}
	});
};
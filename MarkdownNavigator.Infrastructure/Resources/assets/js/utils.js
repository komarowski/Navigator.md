const addOnClickCopyEvent = (element) => {
	element.onclick = () => {
		navigator.clipboard.writeText(element.innerText);
	};
}

const applyCodeCopy = () => {
	const codeElements = document.querySelectorAll("code:not(pre code)");
	codeElements.forEach(code => {
		addOnClickCopyEvent(code)
	});

	const preElements = document.querySelectorAll("pre");
	preElements.forEach(pre => {
		const code = pre.querySelector("code");
		if (code) {
			addOnClickCopyEvent(pre);
		}
	});
};
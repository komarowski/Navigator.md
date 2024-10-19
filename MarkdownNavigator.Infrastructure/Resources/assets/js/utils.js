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

const executeTextareaCode = (textarea) => {
	try {
		new Function(textarea.value)();
	} catch (error) {
		console.error('Error executing code:', error.message);
	}
};

const addTextareaAttributes = (textarea) => {
	const numberOfLines = textarea.value.split('\n').length;
	textarea.setAttribute('rows', numberOfLines);
	textarea.setAttribute('spellcheck', 'false');
	textarea.setAttribute('autocorrect', 'off');
};

const initializeJsTextareas = () => {
	const textareaList = document.querySelectorAll('.textarea-js');
	textareaList.forEach((textarea) => {
		addTextareaAttributes(textarea);
		const button = document.createElement("button");
		button.classList.add("button-run");
		button.addEventListener('click', () => executeTextareaCode(textarea));
		const buttonDiv = document.createElement("div");
		buttonDiv.classList.add("button-run-container");
		buttonDiv.appendChild(button);
		textarea.insertAdjacentElement('beforeBegin', buttonDiv);
	});
};
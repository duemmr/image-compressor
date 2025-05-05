document.addEventListener("DOMContentLoaded", () => {
    const sliderValue       = document.getElementById("sliderValue");
    const fileInput         = document.getElementById("compressInput");
    const compressBtn       = document.getElementById("compressBtn");
    const sliderMode        = document.getElementById("modeQuality");
    const sizeMode          = document.getElementById("modeSize");
    const qualityBlock      = document.getElementById("qualityBlock");
    const targetBlock       = document.getElementById("targetBlock");
    const qualityToggle     = document.getElementById("enableQuality");
    const resizeToggle      = document.getElementById("enableResize");
    const compressionSlider = document.getElementById("compressionSlider");
    const keepAspectRatio   = document.getElementById("keepAspect")
    const dropZone          = document.getElementById("drop-zone");
    const methodSelect      = document.getElementById("methodSelect");
    const widthInput        = document.getElementById("width");
    const heightInput       = document.getElementById("height");
    const form              = document.getElementById("form");

    let width = 0;
    let height = 0;
    let aspectRatio = 0;
    let hasUploaded = false;
    compressBtn.disabled = true;

    keepAspectRatio.addEventListener("change", () => {
        heightInput.disabled = keepAspectRatio.checked;
        if (resizeToggle.checked) updateHeight();
    });

    widthInput.addEventListener("input", () => {
        if (resizeToggle.checked) updateHeight();
    });

    ["dragenter", "dragover"].forEach(eventName => {
        dropZone.addEventListener(eventName, e => {
            e.preventDefault();
            dropZone.classList.add("bg-blue-100");
        });
    });

    ["dragleave", "drop"].forEach(eventName => {
        dropZone.addEventListener(eventName, e => {
            e.preventDefault();
            dropZone.classList.remove("bg-blue-100");
        });
    });

    function buttonEnable() {
        compressBtn.disabled = false;
        compressBtn.classList.remove("opacity-50", "cursor-not-allowed");
        compressBtn.classList.add("cursor-pointer");
    };

    function buttonDisable() {
        compressBtn.disabled = true;
        compressBtn.classList.add("opacity-50", "cursor-not-allowed");
        compressBtn.classList.remove("cursor-pointer");
    };

    compressBtn.addEventListener("click", () => {
        heightInput.disabled = false;
        form.submit();
    });

    dropZone.addEventListener("drop", e => {
        const files = e.dataTransfer.files;
        hasUploaded = files.length;

        if (!files.length) return;
        
        const formData = new FormData();
        formData.append("file", files[0]);
        formData.append("method", methodSelect.value);

        const img = new Image();
        const objectURL = URL.createObjectURL(files[0]);
        img.onload = function() {
            if (!resizeToggle.checked) {
                width = img.naturalWidth;
                height = img.naturalHeight;
                console.log(width, height, "b", img)
            } else {
                width.value = img.naturalWidth;
                height.value = img.naturalHeight;
                console.log(width.value, height.value, "a", Object.assign({}, img))
            };

            aspectRatio = img.naturalWidth / img.naturalHeight;
            
            updateHeight();

            URL.revokeObjectURL(objectURL);
        };

        img.src = objectURL;

        fileInput.files = files;

        if (!resizeToggle.checked && !qualityToggle.checked) return;

        buttonEnable();

        if (compressionSlider) formData.append("quality", compressionSlider.value);
    });

    dropZone.addEventListener("click", () => fileInput.click());

    fileInput.addEventListener("change", e => {
        hasUploaded = e.target.files.length;
        compressBtn.disabled = !(e.target.files.length && (resizeToggle.checked || qualityToggle.checked));
        
        if (!compressBtn.disabled) compressBtn.classList.remove("opacity-50", "cursor-not-allowed");

        const img = new Image();
        const objectURL = URL.createObjectURL(e.target.files[0]);

        img.onload = function() {
            if (!resizeToggle.checked) {
                width = img.naturalWidth;
                height = img.naturalHeight;
                console.log(width, height, "b", img)
            } else {
                width.value = img.naturalWidth;
                height.value = img.naturalHeight;
                console.log(width.value, height.value, "a", Object.assign({}, img))
            };

            aspectRatio = img.naturalWidth / img.naturalHeight;
            
            updateHeight();

            URL.revokeObjectURL(objectURL);
        };

        img.src = objectURL;
    });

    compressionSlider.addEventListener("input", e => sliderValue.textContent = `${e.target.value}%`);

    sliderMode.addEventListener("change", updateMode);
    sizeMode.addEventListener("change", updateMode);

    updateMode();

    qualityToggle.addEventListener("change", () => {
        toggleCover(qualityWrapper, qualityToggle);

        if (resizeToggle.checked) {
            resizeToggle.checked = false;
            toggleCover(resizeWrapper, resizeToggle);
        };

        if (!qualityToggle.checked && !resizeToggle.checked) return buttonDisable();

        if (hasUploaded) buttonEnable();
    });

    resizeToggle.addEventListener("change", () => {
        toggleCover(resizeWrapper, resizeToggle);

        if (resizeToggle.checked) {
            widthInput.value = width;
            heightInput.value = height;
        } else {
            widthInput.value = 0;
            heightInput.value = 0;
        };

        if (qualityToggle.checked) {
            qualityToggle.checked = false;
            toggleCover(qualityWrapper, qualityToggle);
        };

        if (!qualityToggle.checked && !resizeToggle.checked) return buttonDisable();
    
        if (hasUploaded) buttonEnable();
    });

    toggleCover(qualityWrapper, qualityToggle);
    toggleCover(resizeWrapper, resizeToggle);
                                
    heightInput.disabled = resizeToggle.checked;

    updateHeight();

    function updateHeight() {
        if (!resizeToggle.checked) return;

        const width = parseInt(widthInput.value);
        if (!isNaN(width)) heightInput.value = Math.round(width / aspectRatio);
    };

    function updateMode() {
        if (sliderMode.checked) {
            qualityBlock.classList.remove("hidden");
            targetBlock.classList.add("hidden");
        } else {
            qualityBlock.classList.add("hidden");
            targetBlock.classList.remove("hidden");
        };
    };

    function toggleCover(wrapper, toggle) {
        if (toggle.checked) {
            wrapper.classList.add("relative");

            const cover = wrapper.querySelector(".cover");
            cover.classList.add("hidden");
        } else {
            wrapper.classList.add("relative");

            const cover = wrapper.querySelector(".cover");
            cover.classList.remove("hidden");
        };
    };
});
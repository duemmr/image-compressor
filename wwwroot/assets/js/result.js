document.addEventListener("DOMContentLoaded", () => {
    const container = document.querySelector(".slider-container");
    const handle = document.querySelector(".slider-handle");
    const overlay = document.querySelector(".after");
    const modal = document.getElementById("metadataModal");
    const modalContent = document.getElementById("modalContent");
    const closeModal = document.getElementById("closeModal");

    let isDragging = false;

    function updateSlider(x) {
        const rectangle = container.getBoundingClientRect();
        let offset = x - rectangle.left;
        offset = Math.max(0, Math.min(offset, rectangle.width));

        const percent = offset / rectangle.width * 100;

        overlay.style.clip = `rect(0, ${offset}px, ${rectangle.height}px, 0)`;
        handle.style.left = percent + "%";
    }

    updateSlider(window.innerWidth / 2);

    handle.addEventListener("mousedown", () => isDragging = true);
    document.addEventListener("mouseup", () => isDragging = false);

    document.addEventListener("mousemove", (e) => {
        if (isDragging) updateSlider(e.clientX);
    });

    container.addEventListener("click", e => updateSlider(e.clientX));

    function showMetadata(meta) {
        const parsedMeta = JSON.parse(meta);

        modalContent.innerHTML = `
            <p><strong>Format: </strong>${parsedMeta.format ? parsedMeta.format.substring(1) : "Unknown" }</p>
            <p><strong>Size: </strong>${(parsedMeta.size / 1024 / 1024).toFixed(3)} KB</p>
            <p><strong>Dimensions: </strong>${parsedMeta.width}x${parsedMeta.height}</p>`;

        modal.classList.remove("hidden");
        modal.classList.add("flex");
    };

    document.querySelectorAll(".meta-trigger").forEach(btn => {
        btn.addEventListener("click", () => {
            const meta = JSON.parse(btn.getAttribute("data-meta"));
            showMetadata(meta);
        });
    });

    closeModal.addEventListener("click", () => {
        modal.classList.add("hidden");
        modal.classList.remove("flex");
    });
});
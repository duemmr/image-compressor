namespace FileCompressor

open Giraffe.ViewEngine

module Views = 
    let metadataModal =
        div [ _id "metadataModal"; _class "fixed inset-0 bg-[#262424ad] bg-opacity-50 hidden justify-center items-center z-50" ] [
            div [ _class "bg-white p-6 rounded shadow-lg w-full max-w-md relative" ] [
                button [ _id "closeModal"; _class "absolute top-2 right-2 text-gray-500 hover:text-black" ] [ str "✕" ]
                h2 [ _class "text-lg font-bold mb-4" ] [ str "Metadata" ]
                div [ _id "modalContent"; _class "text-sm text-gray-700 space-y-2" ] []
            ]
        ]

    let uploadForm =
        html [] [
            head [] [
                title [] [ str "IC | Home" ]
                link [ _rel "icon"; _type "image/x-icon"; _href "/favicon.ico" ]
                script [
                    _src "https://cdn.jsdelivr.net/npm/@tailwindcss/browser@4.1.5"
                ] []
            ]
            script [] [
                rawText """
                document.addEventListener('DOMContentLoaded', () => {
                    const sliderValue       = document.getElementById('sliderValue');
                    const fileInput         = document.getElementById('compressInput');
                    const compressBtn       = document.getElementById('compressBtn');
                    const sliderMode        = document.getElementById('modeQuality');
                    const sizeMode          = document.getElementById('modeSize');
                    const qualityBlock      = document.getElementById('qualityBlock');
                    const targetBlock       = document.getElementById('targetBlock');
                    const qualityToggle     = document.getElementById('enableQuality');
                    const resizeToggle      = document.getElementById('enableResize');
                    const compressionSlider = document.getElementById('compressionSlider');
                    const keepAspectRatio   = document.getElementById('keepAspect')
                    const dropZone          = document.getElementById('drop-zone');
                    const methodSelect      = document.getElementById('methodSelect');
                    const widthInput        = document.getElementById('width');
                    const heightInput       = document.getElementById('height');
                    const form              = document.getElementById('form');

                    let width = 0;
                    let height = 0;
                    let aspectRatio = 0;
                    let hasUploaded = false;
                    compressBtn.disabled = true;

                    function updateHeight() {
                        if (!keepAspect.checked || !keepAspectRatio.checked) return;

                        const width = parseInt(widthInput.value);
                        if (!isNaN(width)) heightInput.value = Math.round(width / aspectRatio);
                    };

                    keepAspectRatio.addEventListener('change', () => {
                        heightInput.disabled = keepAspect.checked;
                        if (keepAspect.checked) updateHeight();
                    });

                    widthInput.addEventListener('input', () => {
                        if (keepAspect.checked) updateHeight();
                    });

                    ['dragenter', 'dragover'].forEach(eventName => {
                        dropZone.addEventListener(eventName, e => {
                            e.preventDefault();
                            dropZone.classList.add('bg-blue-100');
                        });
                    });

                    ['dragleave', 'drop'].forEach(eventName => {
                        dropZone.addEventListener(eventName, e => {
                            e.preventDefault();
                            dropZone.classList.remove('bg-blue-100');
                        });
                    });

                    function buttonEnable() {
                        compressBtn.disabled = false;
                        compressBtn.classList.remove('opacity-50', 'cursor-not-allowed');
                    };

                    function buttonDisable() {
                        compressBtn.disabled = true;
                        compressBtn.classList.add('opacity-50', 'cursor-not-allowed');
                    };

                    compressBtn.addEventListener('click', () => form.submit());

                    dropZone.addEventListener('drop', e => {
                        const files = e.dataTransfer.files;
                        hasUploaded = files.length;

                        if (!files.length || (!resizeToggle.checked && !qualityToggle.checked)) return;

                        fileInput.files = files;

                        buttonEnable();

                        const formData = new FormData();
                        formData.append("file", files[0]);
                        formData.append("method", methodSelect.value);

                        if (compressionSlider) formData.append("quality", compressionSlider.value);
                    });

                    dropZone.addEventListener('click', () => fileInput.click());

                    fileInput.addEventListener('change', e => {
                        hasUploaded = e.target.files.length;
                        compressBtn.disabled = !(e.target.files.length && (resizeToggle.checked || qualityToggle.checked));
                        
                        if (!compressBtn.disabled) compressBtn.classList.remove('opacity-50','cursor-not-allowed');

                        const img = new Image();
                        const objectURL = URL.createObjectURL(e.target.files[0]);

                        img.onload = function () {
                            if (!keepAspect.checked || !keepAspectRatio.checked) {
                                width = img.naturalWidth;
                                height = img.naturalHeight;
                            } else {
                                width.value = img.naturalWidth;
                                height.value = img.naturalHeight;
                            };

                            aspectRatio = img.naturalWidth / img.naturalHeight;
                            
                            updateHeight();

                            URL.revokeObjectURL(objectURL);
                        };

                        img.src = objectURL;
                    });

                    compressionSlider.addEventListener('input', e => sliderValue.textContent = `${e.target.value}%`);

                    function updateMode() {
                        if (sliderMode.checked) {
                            qualityBlock.classList.remove('hidden');
                            targetBlock.classList.add('hidden');
                        } else {
                            qualityBlock.classList.add('hidden');
                            targetBlock.classList.remove('hidden');
                        };
                    };

                    sliderMode.addEventListener('change', updateMode);
                    sizeMode.addEventListener('change', updateMode);

                    updateMode();

                    function toggleCover(wrapper, toggle) {
                        if (toggle.checked) {
                            wrapper.classList.add('relative');

                            const cover = wrapper.querySelector('.cover');
                            cover.classList.add('hidden');
                        } else {
                            wrapper.classList.add('relative');

                            const cover = wrapper.querySelector('.cover');
                            cover.classList.remove('hidden');
                        };
                    }

                    qualityToggle.addEventListener('change', () => {
                        toggleCover(qualityWrapper, qualityToggle);

                        if (resizeToggle.checked) {
                            resizeToggle.checked = false;
                            toggleCover(resizeWrapper, resizeToggle);
                        };

                        if (!qualityToggle.checked && !resizeToggle.checked) return buttonDisable();

                        if (hasUploaded) buttonEnable();
                    });

                    resizeToggle.addEventListener('change', () => {
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
                                                
                    heightInput.disabled = keepAspect.checked;
                    updateHeight();
                });
                """
            ]
            body [ _class "bg-gray-100 min-h-screen flex items-center justify-center p-4" ] [
                div [] [
                    h1 [ _class "text-2xl font-bold text-center text-gray-800 bg-white p-2 rounded-xl shadow-md w-full max-w-md space-y-3" ] [ str "Compress your images" ]
                    
                    hr [ _class "border-t border-gray-300 my-4" ]

                    form [
                        _method "post"
                        _id "form"
                        _enctype "multipart/form-data"
                        _action "/upload"
                        _accept "image/*"
                        _class "bg-white p-8 rounded-xl shadow-md w-full max-w-md space-y-4"
                        _style "height: 400px; max-height: 400px; overflow-y: scroll;"
                    ] [
                        div [ _id "drop-zone"; _class "border-4 border-dashed border-blue-300 p-6 rounded-lg text-center cursor-pointer hover:bg-blue-50 transition" ] [
                            p [ _class "text-gray-600" ] [ str "Drag and drop an image here, or click to select one" ]
                            input [
                                _type "file"
                                _id "compressInput"
                                _name "file"
                                _class "hidden"
                                _style "display: none"
                            ]
                        ]

                        hr [ _class "border-t border-gray-300 my-4" ]

                        label [ _for "method"; _class "block text-lg font-semibold text-gray-900" ] [ str "Method" ]
                        select [
                            _name "method"
                            _id "methodSelect"
                            _class "cursor-pointer mt-1 block w-full rounded-lg border-2 border-gray-300 focus:ring-2 focus:ring-blue-500 focus:border-blue-500 py-2 px-3"
                        ] [
                            option [ _value "jpeg" ] [ str "JPEG" ]
                            option [ _value "png"  ] [ str "PNG" ]
                            option [ _value "webp" ] [ str "WebP" ]
                        ]

                        hr [ _class "border-t border-gray-300 my-4" ]

                        div [ _class "flex items-center space-x-2"] [
                            label [ _for "enableQuality"; _class "block text-lg font-semibold text-gray-900" ] [ str "Quality" ]
                            input [ _type "checkbox"; _id "enableQuality" ]
                        ]

                        div [ _id "qualityWrapper"; _class "relative" ] [
                            div [ _id "qualityBlock"; _class "space-y-2" ] [
                                div [ _class "flex items-center space-x-4" ] [
                                    input [
                                        _type "range"
                                        _id "compressionSlider"
                                        _name "quality"
                                        _min "0"
                                        _max "100"
                                        _value "50"
                                        _class "w-full"
                                    ]

                                    span [ _id "sliderValue"; _class "text-sm font-medium text-gray-700" ] [ str "50%" ]
                                ]
                            ]

                            div [ _id "targetBlock"; _class "hidden space-y-2" ] [
                                input [
                                    _type "number"; 
                                    _id "targetKb";
                                    _name "targetKb"
                                    _min "0";
                                    _value "0";
                                    _placeholder "100";
                                    _class "mt-1 block w-full rounded-lg border-2 border-gray-300 py-2 px-3"
                                ]
                            ]
    
                            div [ _class "flex space-x-4" ] [
                                label [ _class "block text-lg font-semibold text-gray-900" ] [ str "Mode" ]

                                label [ _class "inline-flex items-center" ] [
                                    input [ _type "radio"; _id "modeQuality"; _name "mode"; _value "quality"; _class "form-radio"; attr "checked" "checked" ]
                                    span [ _class "ml-2 text-gray-700" ] [ str "By quality (%)" ]
                                ]

                                label [ _class "inline-flex items-center" ] [
                                    input [ _type "radio"; _id "modeSize"; _name "mode"; _value "size"; _class "form-radio" ]
                                    span [ _class "ml-2 text-gray-700" ] [ str "By target size (KB)" ]
                                ]
                            ]

                            div [ _class "cover absolute inset-0 bg-white opacity-60 pointer-events-auto z-50" ] []
                        ]

                        hr [ _class "border-t border-gray-300 my-4" ]

                        div [ _class "flex items-center space-x-2"] [
                            label [ _for "enableResize"; _class "block text-lg font-semibold text-gray-900" ] [ str "Resize" ]
                            input [ _type "checkbox"; _id "enableResize"]
                        ]

                        div [ _id "resizeWrapper"; _class "relative" ] [
                            div [ _id "dimensionsBlock"; _class "space-y-2" ] [
                                div [ _class "flex space-x-4" ] [
                                    div [ _class "w-1/2" ] [
                                        input [
                                            _type "number"; 
                                            _id "width";
                                            _name "width"; 
                                            _placeholder "Width";
                                            _min "1"; 
                                            _class "mt-1 block w-full rounded-lg border-2 border-gray-300 py-2 px-3"
                                        ]
                                    ]

                                    div [ _class "w-1/2" ] [
                                        input [
                                            _type "number";
                                            _id "height";
                                            _name "height";
                                            _placeholder "Height";
                                            _min "1";
                                            _class "mt-1 block w-full rounded-lg border-2 border-gray-300 py-2 px-3"
                                        ]
                                    ]
                                ]

                                div [ _class "flex items-center space-x-2" ] [
                                    input [ _type "checkbox"; _id "keepAspect"; _name "keepAspect"; _class "form-checkbox" ]
                                    label [ _for "keepAspect"; _class "text-sm text-gray-700" ] [ str "Keep aspect ratio" ]
                                ]
                            ]

                            div [ _class "cover absolute inset-0 bg-white opacity-60 pointer-events-auto z-50" ] []
                        ]
                    ]

                    hr [ _class "border-t border-gray-300 my-4" ]
                    
                    input [
                        _type "button";
                        _id "compressBtn";
                        _value "💨 GO";
                        _class "w-full bg-blue-600 text-white font-bold py-2 px-4 rounded hover:bg-blue-700 opacity-50 cursor-not-allowed";
                        _disabled
                    ]
                ]
            ]
        ]

    let resultView (beforeUrl: string) (afterUrl: string) (metaBefore: string) (metaAfter: string) =
        html [] [
            head [] [
                title [] [ str "IC | Results" ]
                link [ _rel "icon"; _type "image/x-icon"; _href "/favicon.ico" ]
                script [
                    _src "https://cdn.jsdelivr.net/npm/@tailwindcss/browser@4.1.5"
                ] []
                style [] [
                    rawText """
                    body {
                        font-family: sans-serif;
                        background: #f5f5f5;
                        margin: 0;
                        padding: 2rem;
                        display: flex;
                        justify-content: center;
                    }

                    .wrapper {
                        background: white;
                        padding: 2rem;
                        border-radius: 12px;
                        box-shadow: 0 4px 20px rgba(0,0,0,0.1);
                        max-width: 900px;
                        width: 100%;
                    }

                    .info {
                        display: flex;
                        justify-content: space-between;
                        margin-bottom: 1rem;
                        color: #666;
                        font-size: 0.9rem;
                    }

                    .slider-container {
                        position: relative;
                        width: 100%;
                        height: 500px;
                        overflow: hidden;
                        margin-bottom: 2rem;
                        border-radius: 10px;
                        user-select: none;
                    }

                    .image-wrapper {
                        position: relative;
                        width: 100%;
                        height: 100%;
                    }

                    .image-wrapper img {
                        width: 100%;
                        height: 100%;
                        position: absolute;
                        object-fit: contain;
                        top: 0;
                        left: 0;
                    }

                    .overlay {
                        position: absolute;
                        top: 0;
                        left: 0;
                        width: 50%;
                        height: 100%;
                        overflow: hidden;
                        z-index: 2;
                    }

                    .overlay img {
                        position: absolute;
                        top: 0;
                        left: 0;
                        width: 100%;
                        height: 100%;
                        object-fit: cover;
                    }

                    .slider-handle {
                        position: absolute;
                        top: 0;
                        bottom: 0;
                        left: 50%;
                        width: 4px;
                        background: #007bff;
                        cursor: ew-resize;
                        z-index: 10;
                        transform: translateX(-50%);
                    }

                    .download-btn a {
                        background: #007bff;
                        color: white;
                        text-decoration: none;
                        padding: 0.75rem 1.5rem;
                        border-radius: 8px;
                        font-weight: bold;
                    }

                    .download-btn a:hover {
                        background: #0056b3;
                    }
                    """
                ]
                script [] [
                    rawText """
                    document.addEventListener('DOMContentLoaded', () => {
                        const container = document.querySelector('.slider-container');
                        const handle = document.querySelector('.slider-handle');
                        const overlay = document.querySelector('.after');
                        const modal = document.getElementById('metadataModal');
                        const modalContent = document.getElementById('modalContent');
                        const closeModal = document.getElementById('closeModal');

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

                        handle.addEventListener('mousedown', () => isDragging = true);
                        document.addEventListener('mouseup', () => isDragging = false);

                        document.addEventListener('mousemove', (e) => {
                            if (isDragging) updateSlider(e.clientX);
                        });

                        container.addEventListener('click', e => updateSlider(e.clientX));

                        function showMetadata(meta) {
                            const parsedMeta = JSON.parse(meta);

                            modalContent.innerHTML = `
                                <p><strong>Format: </strong>${parsedMeta.format ? parsedMeta.format.substring(1) : "Unknown" }</p>
                                <p><strong>Size: </strong>${(parsedMeta.size / 1024 / 1024).toFixed(3)} KB</p>
                                <p><strong>Dimensions: </strong>${parsedMeta.width}x${parsedMeta.height}</p>`;

                            modal.classList.remove('hidden');
                            modal.classList.add('flex');
                        };

                        document.querySelectorAll('.meta-trigger').forEach(btn => {
                            btn.addEventListener('click', () => {
                                const meta = JSON.parse(btn.getAttribute('data-meta'));
                                showMetadata(meta);
                            });
                        });

                        closeModal.addEventListener('click', () => {
                            modal.classList.add('hidden');
                            modal.classList.remove('flex');
                        });
                    });
                    """
                ]
            ]
            body [] [
                div [ _class "wrapper" ] [
                    h2 [_class "text-center mb-4 text-4xl font-extrabold leading-none tracking-tight text-gray-900 md:text-5xl lg:text-3xl dark:text-white"] [ 
                        button [
                            _class "meta-trigger underline text-blue-600 cursor-pointer hover:text-blue-800"
                            attr "data-meta" (System.Text.Json.JsonSerializer.Serialize(metaBefore))
                        ] [ str "Before" ]
                        span [] [ str " & "] 
                        button [
                            _class "meta-trigger underline text-blue-600 cursor-pointer hover:text-blue-800"
                            attr "data-meta" (System.Text.Json.JsonSerializer.Serialize(metaAfter))
                        ] [ str "After" ]
                    ]
                    div [ _class "slider-container" ] [
                        div [ _class "border-4 border-gray-100 rounded-lg shadow image-wrapper" ] [
                            img [ _src beforeUrl; _class "before" ]
                            img [ _src afterUrl; _class "after" ]

                            div [ _class "overlay" ] []
                        ]
                        div [ _class "slider-handle" ] []
                    ]
                    div [ _class "flex space-x-4 download-btn justify-center" ] [
                        a [ _href "/"; ] [str "🏠 Home"]
                        a [ _href beforeUrl; attr "download" ""; _target "_blank" ] [str "💾 Download"]
                    ]
                ]

                metadataModal
            ]
        ]

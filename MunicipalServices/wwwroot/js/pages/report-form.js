
// Enhanced file selection handler with multiple file support
function handleFileSelect(input) {
    const fileList = document.getElementById('fileList');
    fileList.innerHTML = '';

    if (input.files.length > 0) {
        const validFiles = [];
        const invalidFiles = [];

        // Validate files and separate valid from invalid
        Array.from(input.files).forEach(file => {
            if (validateFile(file)) {
                validFiles.push(file);
            } else {
                invalidFiles.push(file);
            }
        });

        // Create a new DataTransfer object to hold only valid files
        const dataTransfer = new DataTransfer();
        validFiles.forEach(file => {
            dataTransfer.items.add(file);
        });

        // Replace the input's files with only valid files
        input.files = dataTransfer.files;

        if (validFiles.length > 0) {
            displayValidFiles(validFiles, fileList);
            showFilesSummary(validFiles, fileList);
        }

        if (invalidFiles.length > 0) {
            showFileValidationError(invalidFiles, fileList);
        }
    }

    updateProgress();
}

// Display valid files in an enhanced UI
function displayValidFiles(validFiles, container) {
    const filesContainer = document.createElement('div');
    filesContainer.className = 'selected-files-container';

    // Summary header
    const summaryDiv = document.createElement('div');
    summaryDiv.className = 'alert alert-success d-flex align-items-center mb-3';
    summaryDiv.innerHTML = `
        <i class="fas fa-check-circle me-2"></i>
        <strong>${validFiles.length} file${validFiles.length !== 1 ? 's' : ''} selected successfully!</strong>
        <button type="button" class="btn btn-sm btn-outline-danger ms-auto" onclick="clearAllFiles()">
            <i class="fas fa-trash me-1"></i>Clear All
        </button>
    `;
    container.appendChild(summaryDiv);

    // Individual file list
    const ul = document.createElement('ul');
    ul.className = 'list-group mb-3';

    validFiles.forEach((file, index) => {
        const li = document.createElement('li');
        li.className = 'list-group-item d-flex justify-content-between align-items-center';

        const fileTypeIcon = getFileTypeIcon(file.name);
        const fileSize = (file.size / 1024 / 1024).toFixed(2);

        li.innerHTML = `
            <div class="d-flex align-items-center flex-grow-1">
                <i class="fas fa-${fileTypeIcon.icon} me-3 ${fileTypeIcon.color}" style="font-size: 1.5rem;"></i>
                <div class="file-info">
                    <span class="fw-semibold d-block" title="${file.name}">${truncateFileName(file.name, 40)}</span>
                    <small class="text-muted">${fileSize} MB • ${file.type || 'Unknown type'}</small>
                </div>
            </div>
            <div class="d-flex align-items-center">
                <span class="badge bg-success rounded-pill me-2">
                    <i class="fas fa-check me-1"></i>Valid
                </span>
                <button type="button" class="btn btn-sm btn-outline-danger" onclick="removeFile(${index})" title="Remove file">
                    <i class="fas fa-times"></i>
                </button>
            </div>
        `;
        ul.appendChild(li);
    });

    filesContainer.appendChild(ul);
    container.appendChild(filesContainer);
}

// Show files summary with total size
function showFilesSummary(validFiles, container) {
    const totalSize = validFiles.reduce((total, file) => total + file.size, 0);
    const totalSizeMB = (totalSize / 1024 / 1024).toFixed(2);

    const summaryCard = document.createElement('div');
    summaryCard.className = 'card border-primary';
    summaryCard.innerHTML = `
        <div class="card-body p-3">
            <h6 class="card-title text-primary mb-2">
                <i class="fas fa-info-circle me-2"></i>Upload Summary
            </h6>
            <div class="row text-center">
                <div class="col-4">
                    <div class="text-primary">
                        <i class="fas fa-file-alt fa-2x d-block mb-1"></i>
                        <strong>${validFiles.length}</strong>
                        <small class="d-block text-muted">Files</small>
                    </div>
                </div>
                <div class="col-4">
                    <div class="text-success">
                        <i class="fas fa-weight fa-2x d-block mb-1"></i>
                        <strong>${totalSizeMB} MB</strong>
                        <small class="d-block text-muted">Total Size</small>
                    </div>
                </div>
                <div class="col-4">
                    <div class="text-info">
                        <i class="fas fa-images fa-2x d-block mb-1"></i>
                        <strong>${countFilesByType(validFiles, 'image')}</strong>
                        <small class="d-block text-muted">Images</small>
                    </div>
                </div>
            </div>
        </div>
    `;

    container.appendChild(summaryCard);
}

// Get appropriate icon and color for file type
function getFileTypeIcon(fileName) {
    const extension = fileName.split('.').pop().toLowerCase();

    const iconMap = {
        'jpg': { icon: 'image', color: 'text-success' },
        'jpeg': { icon: 'image', color: 'text-success' },
        'png': { icon: 'image', color: 'text-success' },
        'gif': { icon: 'image', color: 'text-success' },
        'pdf': { icon: 'file-pdf', color: 'text-danger' },
        'doc': { icon: 'file-word', color: 'text-primary' },
        'docx': { icon: 'file-word', color: 'text-primary' },
        'default': { icon: 'file', color: 'text-muted' }
    };

    return iconMap[extension] || iconMap.default;
}

// Truncate long file names
function truncateFileName(fileName, maxLength) {
    if (fileName.length <= maxLength) return fileName;

    const extension = fileName.split('.').pop();
    const nameWithoutExt = fileName.substring(0, fileName.lastIndexOf('.'));
    const truncatedName = nameWithoutExt.substring(0, maxLength - extension.length - 4) + '...';

    return truncatedName + '.' + extension;
}

// Count files by type
function countFilesByType(files, type) {
    return files.filter(file => {
        const mimeType = file.type.toLowerCase();
        return type === 'image' ? mimeType.startsWith('image/') :
            type === 'document' ? (mimeType.includes('pdf') || mimeType.includes('word') || mimeType.includes('document')) :
                false;
    }).length;
}

// Clear all selected files
function clearAllFiles() {
    const fileInput = document.querySelector('input[type="file"]');
    const fileList = document.getElementById('fileList');

    if (fileInput) {
        fileInput.value = '';
        fileInput.files = new DataTransfer().files;
    }

    if (fileList) {
        fileList.innerHTML = '';
    }

    updateProgress();
}

// Enhanced remove individual files function
function removeFile(index) {
    const fileInput = document.querySelector('input[type="file"]');
    const dataTransfer = new DataTransfer();

    // Add all files except the one to remove
    Array.from(fileInput.files).forEach((file, i) => {
        if (i !== index) {
            dataTransfer.items.add(file);
        }
    });

    // Update the input files
    fileInput.files = dataTransfer.files;

    // Refresh the display
    handleFileSelect(fileInput);
}

// Enhanced file validation with detailed error reporting
function validateFile(file) {
    const maxSize = 5 * 1024 * 1024; // 5MB
    const allowedTypes = ['.jpg', '.jpeg', '.png', '.gif', '.pdf', '.doc', '.docx'];

    if (file.size > maxSize) {
        return false;
    }

    const extension = '.' + file.name.split('.').pop().toLowerCase();
    return allowedTypes.includes(extension);
}

// Show detailed file validation errors
function showFileValidationError(invalidFiles, container) {
    const errorDiv = document.createElement('div');
    errorDiv.className = 'alert alert-warning mt-2';

    let errorMessages = [];
    const maxSize = 5 * 1024 * 1024; // 5MB
    const allowedTypes = ['.jpg', '.jpeg', '.png', '.gif', '.pdf', '.doc', '.docx'];

    invalidFiles.forEach(file => {
        const extension = '.' + file.name.split('.').pop().toLowerCase();
        if (file.size > maxSize) {
            errorMessages.push(`${file.name}: File too large (${(file.size / 1024 / 1024).toFixed(2)} MB, max 5MB)`);
        } else if (!allowedTypes.includes(extension)) {
            errorMessages.push(`${file.name}: Unsupported file type (${extension})`);
        }
    });

    errorDiv.innerHTML = `
        <div class="d-flex align-items-start">
            <i class="fas fa-exclamation-triangle me-2 mt-1"></i>
            <div>
                <strong>The following files were rejected:</strong>
                <ul class="mb-0 mt-2">
                    ${errorMessages.map(msg => `<li class="small">${msg}</li>`).join('')}
                </ul>
                <small class="text-muted d-block mt-2">
                    Supported formats: JPG, PNG, GIF, PDF, DOC, DOCX (max 5MB each)
                </small>
            </div>
        </div>
    `;

    container.appendChild(errorDiv);
}

// Enhanced drag and drop for multiple files
function initializeFileUpload() {
    const fileUploadArea = document.getElementById('fileUploadArea');
    const fileInput = document.querySelector('input[type="file"]');

    if (!fileUploadArea || !fileInput) return;

    // Drag and drop functionality for multiple files
    fileUploadArea.addEventListener('dragenter', (e) => {
        e.preventDefault();
        fileUploadArea.classList.add('drag-over');
    });

    fileUploadArea.addEventListener('dragover', (e) => {
        e.preventDefault();
        fileUploadArea.classList.add('drag-over');
    });

    fileUploadArea.addEventListener('dragleave', (e) => {
        e.preventDefault();
        if (!fileUploadArea.contains(e.relatedTarget)) {
            fileUploadArea.classList.remove('drag-over');
        }
    });

    fileUploadArea.addEventListener('drop', (e) => {
        e.preventDefault();
        fileUploadArea.classList.remove('drag-over');

        const files = Array.from(e.dataTransfer.files);
        if (files.length > 0) {
            // Merge with existing files if any
            const existingFiles = Array.from(fileInput.files || []);
            const allFiles = [...existingFiles, ...files];

            // Create a new DataTransfer object with all files
            const dataTransfer = new DataTransfer();
            allFiles.forEach(file => {
                dataTransfer.items.add(file);
            });

            // Set the files to the input and process them
            fileInput.files = dataTransfer.files;
            handleFileSelect(fileInput);
        }
    });
}

// Enhanced progress tracking
function updateProgress() {
    const location = document.querySelector('input[name="Location"]')?.value.trim() || '';
    const category = document.querySelector('select[name="Category"]')?.value || '';
    const description = document.querySelector('textarea[name="Description"]')?.value.trim() || '';
    const files = document.querySelector('input[type="file"]')?.files || [];

    let progress = 0;

    if (location && location.length >= 5) progress += 25;
    if (category) progress += 25;
    if (description && description.length >= 10) progress += 30;
    if (files.length > 0) progress += 20;

    // Update progress bar
    const progressBar = document.querySelector('.engagement-progress');
    if (progressBar) {
        progressBar.style.width = progress + '%';
    }

    // Update messages
    const engagementMessages = [
        "Your report helps improve the community for everyone!",
        "Thank you for being an active community member!",
        "Together, we can make our municipality better!",
        "Your voice matters in building a better community!"
    ];

    if (progress > 0) {
        const messageIndex = Math.floor(progress / 25);
        const messageElement = document.getElementById('engagementMessage');
        if (messageElement) {
            messageElement.textContent =
                engagementMessages[Math.min(messageIndex, engagementMessages.length - 1)];
        }
    }

    updateFormValidation();
}

// Form validation with file check
function updateFormValidation() {
    const location = document.querySelector('input[name="Location"]')?.value.trim() || '';
    const category = document.querySelector('select[name="Category"]')?.value || '';
    const description = document.querySelector('textarea[name="Description"]')?.value.trim() || '';

    const isValid = location.length >= 5 &&
        category &&
        description.length >= 10;

    const submitBtn = document.getElementById('submitBtn');
    if (submitBtn) {
        submitBtn.disabled = !isValid;

        if (isValid) {
            submitBtn.classList.remove('btn-secondary');
            submitBtn.classList.add('btn-primary');
        } else {
            submitBtn.classList.remove('btn-primary');
            submitBtn.classList.add('btn-secondary');
        }
    }
}

// Form submission with final validation
function handleFormSubmission(event) {
    const form = event.target;
    const fileInput = form.querySelector('input[type="file"]');
    const submitBtn = form.querySelector('#submitBtn');

    // Final validation of all files before submission
    if (fileInput && fileInput.files.length > 0) {
        const invalidFiles = Array.from(fileInput.files).filter(file => !validateFile(file));
        if (invalidFiles.length > 0) {
            event.preventDefault();

            // Remove any existing error alerts
            const existingAlerts = form.querySelectorAll('.alert-danger');
            existingAlerts.forEach(alert => alert.remove());

            // Show error message
            const errorAlert = document.createElement('div');
            errorAlert.className = 'alert alert-danger alert-dismissible fade show';
            errorAlert.innerHTML = `
                <i class="fas fa-exclamation-circle me-2"></i>
                <strong>Cannot submit:</strong> Please remove invalid files before submitting.
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            `;

            // Insert at the top of the form
            form.insertBefore(errorAlert, form.firstChild);

            // Scroll to error
            errorAlert.scrollIntoView({ behavior: 'smooth' });

            // Re-enable submit button
            if (submitBtn) {
                submitBtn.disabled = false;
                submitBtn.innerHTML = '<i class="fas fa-paper-plane me-2"></i>Submit Report';
            }

            return false;
        }
    }

    // If validation passes, show loading state
    if (submitBtn && !submitBtn.disabled) {
        submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Submitting...';
        submitBtn.disabled = true;
    }

    return true;
}

// Character counter for description
function updateCharacterCount(textarea) {
    const charCount = textarea.value.length;
    const charCountElement = document.getElementById('charCount');

    if (charCountElement) {
        charCountElement.textContent = `${charCount}/1000 characters`;

        // Change color based on length
        if (charCount < 10) {
            charCountElement.className = 'text-danger';
        } else if (charCount > 900) {
            charCountElement.className = 'text-warning';
        } else {
            charCountElement.className = 'text-muted';
        }
    }

    updateProgress();
}

// Individual field validation
function validateCategory(select) {
    const feedback = select.nextElementSibling;
    if (select.value === '') {
        select.classList.add('is-invalid');
        if (feedback && feedback.classList.contains('text-danger')) {
            feedback.textContent = 'Please select a category.';
        }
    } else {
        select.classList.remove('is-invalid');
        select.classList.add('is-valid');
    }
}

function validateDescription(textarea) {
    const feedback = textarea.nextElementSibling;
    if (textarea.value.trim().length < 10) {
        textarea.classList.add('is-invalid');
        if (feedback && feedback.classList.contains('text-danger')) {
            feedback.textContent = 'Description must be at least 10 characters long.';
        }
    } else {
        textarea.classList.remove('is-invalid');
        textarea.classList.add('is-valid');
    }
}

// Initialize everything when DOM is loaded
document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('reportForm');

    if (form) {
        // Initialize file upload functionality
        initializeFileUpload();

        // Initialize form state
        updateFormValidation();

        // Add real-time validation
        form.addEventListener('input', updateFormValidation);
        form.addEventListener('change', updateFormValidation);

        // Add form submission handler
        form.addEventListener('submit', handleFormSubmission);

        // Initialize character count if description has content
        const descriptionTextarea = form.querySelector('textarea[name="Description"]');
        if (descriptionTextarea) {
            if (descriptionTextarea.value) {
                updateCharacterCount(descriptionTextarea);
            }

            // Add input event listener for real-time character counting
            descriptionTextarea.addEventListener('input', function () {
                updateCharacterCount(this);
            });
        }
    }
});
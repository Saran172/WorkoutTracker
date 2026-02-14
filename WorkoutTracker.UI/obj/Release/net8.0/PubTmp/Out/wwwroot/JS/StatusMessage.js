window.NotificationFx = (function () {
    // Inject CSS dynamically
    const styles = `
        .notify-container {
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 9999;
            display: flex;
            flex-direction: column;
            gap: 10px;
        }
        .notify {
            min-width: 280px;
            max-width: 400px;
            padding: 15px 20px;
            border-radius: 10px;
            color: #fff;
            font-family: system-ui, sans-serif;
            box-shadow: 0 5px 15px rgba(0,0,0,0.2);
            opacity: 0;
            transform: translateY(-20px);
            animation: slideIn 0.4s forwards;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }
        .notify.success {
            background: linear-gradient(135deg, #28a745, #218838);
        }
        .notify.error {
            background: linear-gradient(135deg, #dc3545, #b02a37);
        }
        .notify h4 {
            margin: 0 0 5px 0;
            font-size: 1rem;
        }
        .notify p {
            margin: 0;
            font-size: 0.9rem;
        }
        .notify button {
            background: transparent;
            border: none;
            color: #fff;
            font-size: 1rem;
            cursor: pointer;
            margin-left: 10px;
        }
        @keyframes slideIn {
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }
        @keyframes fadeOut {
            to {
                opacity: 0;
                transform: translateY(-20px);
            }
        }
    `;

    const styleSheet = document.createElement("style");
    styleSheet.innerText = styles;
    document.head.appendChild(styleSheet);

    // Create container once
    let container = document.querySelector(".notify-container");
    if (!container) {
        container = document.createElement("div");
        container.className = "notify-container";
        document.body.appendChild(container);
    }

    // Main function to create notifications
    function show(type, title, message, duration = 3000) {
        const div = document.createElement("div");
        div.className = `notify ${type}`;
        div.innerHTML = `
            <div>
                <h4>${title}</h4>
                <p>${message}</p>
            </div>
        `;

        container.appendChild(div);

        // Remove notification on click or timeout
        const remove = () => {
            div.style.animation = "fadeOut 0.3s forwards";
            setTimeout(() => div.remove(), 300);
        };

        //div.querySelector("button").addEventListener("click", remove);
        setTimeout(remove, duration);
    }

    return {
        success: (msg) => show("success", "Success!", msg),
        error: (msg) => show("error", "Error!", msg)
    };
})();

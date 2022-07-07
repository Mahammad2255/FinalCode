document.querySelector(".minus-btn").setAttribute("disabled", "disabled");

var valueCount;

document.querySelector(".plus-btn").addEventListener("click", function () {
  valueCount = document.getElementById("quantity").value;
  valueCount++;
  document.getElementById("quantity").value = valueCount;

  if (valueCount > 1) {
    document.querySelector(".minus-btn").removeAttribute("disabled");
    document.querySelector(".minus-btn").classList.remove("disabled");
  }
  if (valueCount < 0) {
    valueCount = document.getElementById("quantity").value;

    document.getElementById("quantity").value = 1;
  }
});

document.querySelector(".minus-btn").addEventListener("click", function () {
  valueCount = document.getElementById("quantity").value;
  valueCount--;
  document.getElementById("quantity").value = valueCount;

  if (valueCount == 1) {
    document.querySelector(".minus-btn").setAttribute("disabled", "disabled");
  }
});



// CREDITS : https://www.cssscript.com/image-zoom-pan-hover-detail-view/
var addZoom = (target) => {
    // (A) GET CONTAINER + IMAGE SOURCE
    let container = document.getElementById(target),
        imgsrc = container.currentStyle || window.getComputedStyle(container, false);
        imgsrc = imgsrc.backgroundImage.slice(4, -1).replace(/"/g, "");
   
    // (B) LOAD IMAGE + ATTACH ZOOM
    let img = new Image();
    img.src = imgsrc;
    img.onload = () => {
      // (B1) CALCULATE ZOOM RATIO
      let ratio = img.naturalHeight / img.naturalWidth,
          percentage = ratio * 100 + "%";
   
      // (B2) ATTACH ZOOM ON MOUSE MOVE
      container.onmousemove = (e) => {
        let rect = e.target.getBoundingClientRect(),
            xPos = e.clientX - rect.left,
            yPos = e.clientY - rect.top,
            xPercent = xPos / (container.clientWidth / 100) + "%",
            yPercent = yPos / ((container.clientWidth * ratio) / 100) + "%";
   
        Object.assign(container.style, {
          backgroundPosition: xPercent + " " + yPercent,
          backgroundSize: img.naturalWidth + "px"
        });
      };
   
      // (B3) RESET ZOOM ON MOUSE LEAVE
      container.onmouseleave = (e) => {
        Object.assign(container.style, {
          backgroundPosition: "center",
          backgroundSize: "cover"
        });
      };
    }
  };
   
  // (C) ATTACH FOLLOW ZOOM
  window.onload = () => { addZoom("zoomC"); };






 
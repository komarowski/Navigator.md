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

setSlider();
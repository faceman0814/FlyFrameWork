const cubic = (value: number): number => Math.pow(value, 3)
const easeInOutCubic = (value: number): number => value < 0.5 
  ? cubic(value * 2) / 2 
  : 1 - cubic((1 - value) * 2) / 2

export function scrollTo(to: number, duration: number): void {
  const element = document.documentElement
  const start = element.scrollTop
  const change = to - start
  const increment = 20
  let currentTime = 0

  const animateScroll = (): void => {
    currentTime += increment
    const val = easeInOutCubic(currentTime / duration)
    element.scrollTop = start + change * val
    if (currentTime < duration) {
      setTimeout(animateScroll, increment)
    }
  }
  
  animateScroll()
}
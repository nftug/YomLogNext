'use strict'

// Back to close (on top page)
window.registerPageInfoService = (pageInfoService) => {
  window.pageContainer = pageInfoService
  window.addEventListener('popstate', () =>
    window.pageContainer?.invokeMethodAsync('OnPopState')
  )
}

// Scroll
window.onload = () => {
  // Since app manages scroll position, scrollRestoration should set to 'manual'
  history.scrollRestoration = 'manual'
}

window.registerScrollInfoService = (scrollInfoService) => {
  if (!window.scrollInfoService) {
    window.scrollInfoService = scrollInfoService
  }

  window.addEventListener('popstate', () => {
    window.scrollInfoService?.invokeMethodAsync('OnPopState')
  })
}

window.onscroll = () => {
  let scrollY = window.scrollY
  const maxScrollY = document.documentElement.scrollHeight - window.innerHeight

  // スクロール位置がTop/Bottomの時は、自動で勝手にスクロールしている場合がある
  // →Top/Bottomの際はBlazor側にイベントを伝えないようにする
  const isTop = scrollY == 0
  const isBottom = maxScrollY - scrollY <= 2
  if (isTop || isBottom) return

  if (scrollY < 5) scrollY = 0
  else if (maxScrollY - scrollY < 5) scrollY = maxScrollY

  window.scrollInfoService?.invokeMethodAsync('OnScroll', scrollY)
}

window.setScrollY = (scrollY) => {
  window.scrollTo(0, scrollY)
}

// Timeline
window.getActiveElementTagName = () => document.activeElement.tagName

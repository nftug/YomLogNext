'use strict'

// Back to close (on top page)
window.onGoBackOnTopPage = () => {
  if (window.showingDialog) return
  window.pageContainer?.invokeMethodAsync('GoBackOnTopPage')
}

window.onRenderTopPage = (pageContainer) => {
  history.pushState(null, null, location.href)
  window.pageContainer = pageContainer
  window.addEventListener('popstate', window.onGoBackOnTopPage)
}

window.onLeaveTopPage = () => {
  window.pageContainer = null
  window.removeEventListener('popstate', window.onGoBackOnTopPage)
}

// Dialog
window.onOpenDialog = () => {
  window.showingDialog = true
  // When showing dialog, scrollRestoration should set to 'auto'
  history.scrollRestoration = 'auto'
  history.pushState('dialog', null, location.href)
}

window.onRemoveDialog = () => {
  window.showingDialog = false
  // Recover scrollRestoration
  history.scrollRestoration = 'manual'
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
    if (window.showingDialog) return
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

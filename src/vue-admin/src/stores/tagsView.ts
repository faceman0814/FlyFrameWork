import { defineStore } from 'pinia'
import { RouteLocationNormalized } from 'vue-router'

interface TagsViewState {
  visitedViews: RouteLocationNormalized[]
  cachedViews: string[]
}

export const useTagsViewStore = defineStore('tagsView', {
  state: (): TagsViewState => ({
    visitedViews: [],
    cachedViews: []
  }),

  getters: {
    getVisitedViews: (state) => state.visitedViews,
    getCachedViews: (state) => state.cachedViews
  },

  actions: {
    addView(view: RouteLocationNormalized) {
      this.addVisitedView(view)
      this.addCachedView(view)
    },

    addVisitedView(view: RouteLocationNormalized) {
      if (this.visitedViews.some(v => v.path === view.path)) return
      this.visitedViews.push(
        Object.assign({}, view, {
          title: view.meta?.title || 'no-name'
        })
      )
    },

    addCachedView(view: RouteLocationNormalized) {
      if (this.cachedViews.includes(view.name as string)) return
      if (view.meta?.noCache) return
      this.cachedViews.push(view.name as string)
    },

    delView(view: RouteLocationNormalized) {
      return new Promise(resolve => {
        this.delVisitedView(view)
        this.delCachedView(view)
        resolve({
          visitedViews: [...this.visitedViews],
          cachedViews: [...this.cachedViews]
        })
      })
    },

    delVisitedView(view: RouteLocationNormalized) {
      for (const [i, v] of this.visitedViews.entries()) {
        if (v.path === view.path) {
          this.visitedViews.splice(i, 1)
          break
        }
      }
    },

    delCachedView(view: RouteLocationNormalized) {
      const index = this.cachedViews.indexOf(view.name as string)
      index > -1 && this.cachedViews.splice(index, 1)
    },

    updateVisitedView(view: RouteLocationNormalized) {
      for (let v of this.visitedViews) {
        if (v.path === view.path) {
          v = Object.assign(v, view)
          break
        }
      }
    },

    delOthersViews(view: RouteLocationNormalized) {
      return new Promise(resolve => {
        this.visitedViews = this.visitedViews.filter(v => {
          return v.meta?.affix || v.path === view.path
        })
        this.cachedViews = this.cachedViews.filter(name => {
          const currentView = this.visitedViews.find(v => v.name === name)
          return currentView && (currentView.meta?.affix || currentView.path === view.path)
        })
        resolve({
          visitedViews: [...this.visitedViews],
          cachedViews: [...this.cachedViews]
        })
      })
    },

    delAllViews() {
      return new Promise(resolve => {
        this.visitedViews = this.visitedViews.filter(tag => tag.meta?.affix)
        this.cachedViews = []
        resolve({
          visitedViews: [...this.visitedViews],
          cachedViews: [...this.cachedViews]
        })
      })
    }
  }
})
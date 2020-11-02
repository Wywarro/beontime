<template>
  <aside
    :class="isOpen ? 'translate-x-0' : '-translate-x-full'">
  </aside>
</template>

<script>
import { defineComponent } from "vue";

export default defineComponent({
  name: "NavigationDrawer",
  props: {
    isOpen: {
      type: Boolean,
      required: true,
      default: false,
    },
  },
  watch: {
    isOpen: {
      immediate: true,
      handler (isOpen) {
        if (process.client) {
          if (isOpen) document.body.style.setProperty("overflow", "hidden");
          else document.body.style.removeProperty("overflow");
        }
      },
    },
  },
});
</script>

<style scoped lang="postcss">
.topbar-nav {
  @apply transform top-0 left-0 w-64;
  @apply bg-white fixed h-full overflow-auto;
  @apply ease-in-out transition-all duration-300 z-30;
}
</style>

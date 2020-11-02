<template>
  <aside
    class="transform navigation-drawer"
    :class="drawerOpened ? 'translate-x-0' : '-translate-x-full'">
    <p>dobrze, ze jestes</p>
  </aside>
</template>

<script>
import { defineComponent } from "vue";

export default defineComponent({
  name: "NavigationDrawer",
  props: {
    drawerOpened: {
      type: Boolean,
      required: true,
      default: false,
    },
  },
});
</script>

<style scoped lang="postcss">
.navigation-drawer {
  margin-top: 4rem;

  @apply top-0 left-0 w-64;
  @apply bg-red-700 fixed h-full overflow-auto;
  @apply ease-in-out transition-all duration-300 z-30;
}
</style>

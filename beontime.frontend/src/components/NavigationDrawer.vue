<template>
  <aside
    class="transform navigation-drawer"
    :class="drawerOpened ? 'translate-x-0' : '-translate-x-full'">
    <ul class="navigation">
      <template v-for="link in navigation" :key="link.name">
        <router-link
          class="navigation-item"
          :to="{ name: link.name }"
          v-slot="{ href, route, navigate, isActive, isExactActive }"
        >
          <li
            :class="[isActive && 'router-link-active', isExactActive && 'router-link-exact-active']"
          >
            <a :href="href" @click="navigate">{{ route.name }}</a>
          </li>
        </router-link>
      </template>
    </ul>
  </aside>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { NavigationItem } from "@/types/types";

export default defineComponent({
  name: "NavigationDrawer",
  props: {
    drawerOpened: {
      type: Boolean,
      required: true,
      default: false,
    },
  },
  data: () => ({
    navigation: [
      { name: "Home", },
      { name: "dajesz", },
      { name: "no jasne", },
    ] as NavigationItem[],
  }),
});
</script>

<style scoped lang="postcss">

</style>

<style scoped lang="less">
.navigation-drawer {
  @apply mt-16;
  @apply top-0;
  @apply left-0;
  @apply w-64;
  @apply bg-teal-500;
  @apply fixed;
  @apply h-full;
  @apply overflow-auto;
  @apply ease-in-out;
  @apply transition-all;
  @apply duration-300;
  @apply z-30;
}

.navigation {
  @apply flex;
  @apply flex-col;

  &-item {
    @apply p-3;
    @apply mx-4;
    @apply my-2;
    @apply rounded-md;
    @apply: text-white;
    @apply bg-gray-600;
    @apply bg-opacity-50;

    &:first-child {
      @apply mt-4;
    }
  }
}
</style>

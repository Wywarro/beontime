<template>
  <aside
    class="transform nav-drawer"
    :class="drawerOpened ? 'translate-x-0' : '-translate-x-full'"
  >
    <ul class="nav">
      <router-link
        v-for="link in navigation"
        v-slot="{ href, navigate, isActive, isExactActive }"
        :key="link.name"
        class="nav__item"
        :to="{ name: link.viewName }"
      >
        <li
          :class="[
            isActive && 'router-link-active',
            isExactActive && 'router-link-exact-active'
          ]"
        >
          <font-awesome-icon :icon="link.icon" class="nav__icon" />
          <a :href="href" @click="navigate">{{ link.title }}</a>
        </li>
      </router-link>
    </ul>
  </aside>
</template>

<script lang="ts">
import { Options, Vue } from "vue-class-component";
import NavigationItem from "@/types/NavigationItem";

@Options({
  props: {
    drawerOpened: {
      type: Boolean,
      required: true,
      default: false
    }
  }
})
export default class NavigationDrawer extends Vue {
  navigation: NavigationItem[] = [
    { title: "Home", icon: "home", viewName: "home" },
    { title: "Work Calendar", icon: "calendar-day", viewName: "work-calendar" },
    { title: "Overtime status", icon: "receipt", viewName: "overtime-status" }
  ];
}
</script>

<style scoped lang="less">
.nav-drawer {
  @apply mt-16;
  @apply top-0;
  @apply left-0;
  @apply w-64;
  @apply bg-green-500;
  @apply fixed;
  @apply h-full;
  @apply overflow-auto;
  @apply ease-in-out;
  @apply transition-all;
  @apply duration-300;
  @apply z-30;
}

.nav {
  display: flex;
  flex-direction: column;

  &__item {
    @apply p-3;
    @apply mx-3;
    @apply my-1;
    @apply rounded-md;
    @apply text-white;
    @apply transition-all;
    @apply duration-300;
    @apply text-base;

    &:first-child {
      @apply mt-4;
    }

    &:hover {
      color: aliceblue;
      @apply bg-gray-700;
      @apply bg-opacity-50;
    }

    &--list-item {
      @apply transition-all;
      @apply duration-100;
    }
  }

  &__icon {
    @apply h-6;
    width: 25%;
    @apply pr-5;
    display: inline-block;
  }
}

.router-link-active {
  @apply text-gray-800;
  @apply font-medium;
}
</style>

<template>
  <component
    :is="componentTag"
    class="be-button"
    :to="to"
    :href="href"
    :color="color"
    :disabled="disabled"
  >
    <font-awesome-icon v-if="icon" :icon="icon" class="be-button__icon" />
    <span class="be-button__value">
      <slot />
    </span>
  </component>
</template>

<script lang="ts">
import { PropType } from "vue";
import { Options, Vue } from "vue-class-component";

interface Tags {
  link: () => string;
  anchor: () => string;
  button: () => string;
}

@Options({
  props: {
    href: {
      type: String,
      default: null
    },
    to: {
      type: String,
      default: null
    },
    disabled: {
      type: Boolean,
      default: false
    },
    type: {
      type: String as PropType<string>,
      default: "button"
    },
    color: {
      type: String,
      default: "primary"
    },
    icon: {
      type: String,
      default: ""
    }
  }
})
export default class BeButton extends Vue {
  href!: string;
  to!: string;
  disabled!: boolean;
  type!: string;
  color!: string;
  icon!: string;

  loading = false;

  get componentTag(): string {
    const tags = {
      link: () => "router-link",
      anchor: () => "a",
      button: () => "button"
    } as Tags;

    return tags[this.type as keyof Tags]();
  }
}
</script>

<style lang="less" scoped>
.be-button {
  cursor: pointer;
  border: none;
  background: 0 0;
  @apply text-base;
  @apply rounded-md;
  text-decoration: none;

  padding: 6px 24px;
  outline: 0;

  user-select: none;
  display: flex;
  justify-content: center;
  align-items: center;

  @apply transition;
  @apply duration-200;

  &::before,
  &::after {
    content: "";
    flex: 1 0 auto;
  }

  &:disabled {
    cursor: not-allowed;
    opacity: 0.333;
  }

  &[color="primary"] {
    @apply bg-blue-600;
    @apply text-white;

    &:hover {
      @apply bg-blue-700;
    }
  }

  &[color="secondary"] {
    @apply bg-indigo-600;
    @apply text-white;

    &:hover {
      @apply bg-indigo-700;
    }
  }

  &[color="success"] {
    @apply bg-green-600;
    @apply text-white;

    &:hover {
      @apply bg-green-700;
    }
  }

  &[color="warning"] {
    @apply bg-yellow-600;
    @apply text-white;

    &:hover {
      @apply bg-yellow-700;
    }
  }

  &[color="danger"] {
    @apply bg-red-600;
    @apply text-white;

    &:hover {
      @apply bg-red-700;
    }
  }
}
</style>

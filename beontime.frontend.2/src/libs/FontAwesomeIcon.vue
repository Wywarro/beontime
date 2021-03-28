<template>
  <svg
    xmlns="http://www.w3.org/2000/svg"
    :class="$props.class"
    :viewBox="`0 0 ${width} ${height}`"
  >
    <path fill="currentColor" :d="svgPath" />
  </svg>
</template>

<script lang="ts">
import {
  findIconDefinition,
  IconName,
  IconPrefix
} from "@fortawesome/fontawesome-svg-core";

import { Options, Vue } from "vue-class-component";

@Options({
  props: {
    icon: {
      type: String,
      required: true
    },
    type: {
      type: String,
      default: "fas",
      required: false
    },
    class: String
  }
})
export default class FontAwesomeIcon extends Vue {
  icon!: string;
  type!: string;
  class!: string;

  get definition() {
    return findIconDefinition({
      prefix: this.type as IconPrefix,
      iconName: this.icon as IconName
    });
  }

  get width() {
    return this.definition.icon[0];
  }

  get height() {
    return this.definition.icon[1];
  }

  get svgPath() {
    return this.definition.icon[4];
  }
}
</script>

<template>
<div class="cd-schedule cd-schedule--loading margin-top-lg margin-bottom-lg js-cd-schedule">
  <div class="cd-schedule__timeline">
    <ul>
      <li><span>09:00</span></li>
      <li><span>09:30</span></li>
      <!-- additional elements here -->
    </ul>
  </div> <!-- .cd-schedule__timeline -->

  <div class="cd-schedule__events">
    <ul>
      <li class="cd-schedule__group">
        <div class="cd-schedule__top-info"><span>Monday</span></div>

        <ul>
          <li class="cd-schedule__event">
            <a data-start="09:30" data-end="10:30" data-content="event-abs-circuit" data-event="event-1" href="#0">
              <em class="cd-schedule__name">Abs Circuit</em>
            </a>
          </li>

          <!-- other events here -->
        </ul>
      </li>

      <li class="cd-schedule__group">
        <div class="cd-schedule__top-info"><span>Tuesday</span></div>

        <ul>
          <!-- events here -->
        </ul>
      </li>

      <!-- additional li.cd-schedule__group here -->
    </ul>
  </div>

  <div class="cd-schedule-modal">
    <header class="cd-schedule-modal__header">
      <div class="cd-schedule-modal__content">
        <span class="cd-schedule-modal__date"></span>
        <h3 class="cd-schedule-modal__name"></h3>
      </div>

      <div class="cd-schedule-modal__header-bg"></div>
    </header>

    <div class="cd-schedule-modal__body">
      <div class="cd-schedule-modal__event-info"></div>
      <div class="cd-schedule-modal__body-bg"></div>
    </div>

    <a href="#0" class="cd-schedule-modal__close text-replace">Close</a>
  </div>
</div> <!-- .cd-schedule -->
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({

});
</script>

<style lang="less" scoped>
.cd-schedule__group > ul {
  position: relative;
  padding: 0 var(--component-padding);
  display: flex;
  overflow-x: scroll;
  -webkit-overflow-scrolling: touch;
}

.cd-schedule__event {
  flex-shrink: 0; // force them to stay on one line
  float: left; // flex fallback
  height: 150px;
  width: 70%;
  max-width: 300px;
}

.cd-schedule-modal {
  position: fixed;
  z-index: 3;
  top: 0;
  right: 0;
  height: 100%;
  width: 100%;
  visibility: hidden;
  transform: translateX(100%);
  transition: transform .4s, visibility .4s;
}

.cd-schedule-modal--open { // this class is added as soon as an event is selected
  transform: translateX(0);
  visibility: visible;
}
</style>

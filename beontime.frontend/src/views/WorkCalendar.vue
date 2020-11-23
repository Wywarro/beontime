<template>
    <div class="cal__container">
        <div class="cal__title">Hello!</div>
        <div class="cal__days">
            <div />
            <div />
            <div class="cal__day">{{ getMonday(new Date()) }}</div>
            <div class="cal__day">Mon</div>
            <div class="cal__day">Mon</div>
            <div class="cal__day">Mon</div>
            <div class="cal__day">Mon</div>
            <div class="cal__day">Mon</div>
            <div class="cal__day">Mon</div>
        </div>
        <div class="cal__content">
            <div
                v-for="hour in hours"
                :key="hour"
                class="cal__time"
                :style="{ 'grid-row': hour }"
            >
                {{ hourTime(hour) }}
            </div>
            <div class="cal__filler-col" />
            <div
                v-for="day in days"
                :key="`10${day}`"
                class="cal__col"
                :style="{ 'grid-column': day }"
            />
            <div
                v-for="(hour, index) in hours"
                :key="`20${hour}`"
                class="cal__row"
                :style="{ 'grid-row': index + 1 }"
            />
            <div class="cal__current-time">
                <div class="cal__circle" />
            </div>
        </div>
    </div>
</template>

<script lang="ts">
import { defineComponent, ref } from "vue";

import startOfWeek from "date-fns/start_of_week";
import { range } from "lodash";

export default defineComponent({
    setup() {
        const hours = ref(range(1, 24));

        const days = ref(range(3, 10));

        const hourTime = (number: number) => {
            if (number < 10) {
                return `0${number}:00`;
            }
            return `${number}:00`;
        };

        const getMonday = (date: Date) => {
            date = new Date(date);
            const dayOfWeek = date.getDay();
            const sundayAdjustment = dayOfWeek === 0 ? -6 : 1;
            const diff = date.getDate() - dayOfWeek + sundayAdjustment; // adjust when day is sunday

            const monday = new Date(date.setDate(diff));

            const options = { weekday: "long", month: "long", day: "numeric" };
            return monday.toLocaleDateString("pl-PL", options);
        };

        return { hours, days, hourTime, getMonday };
    },
});
</script>

<style lang="less" scoped>
@title-height: 3em;
@days-height: 3em;
@time-width: 3em;
@time-height: 3em;
@grid-color: #dadce0;
@calendar-template: @time-width 10px repeat(7, 1fr);
@current-time-color: #ea4335;

.cal {
    &__container {
        width: 100%;
        display: grid;
        grid-template-rows: @title-height @days-height auto;
        position: absolute;
    }

    &__title {
        background: #217346;
        text-align: center;
        display: grid;
        place-content: center;
        color: #fff;
        top: 0;
        z-index: 10;
    }

    &__days {
        background: #f3f2f1;
        display: grid;
        place-content: center;
        text-align: center;
        grid-template-columns: @calendar-template;
        top: @title-height;
        z-index: 10;
        border-bottom: 2px solid @grid-color;
    }

    &__day {
        border-left: 1px solid @grid-color;
    }

    &__content {
        display: grid;
        grid-template-columns: @calendar-template;
        grid-template-rows: repeat(24, @time-height);
    }

    &__time {
        grid-column: 1;
        text-align: right;
        align-self: end;
        font-size: 80%;
        position: relative;
        bottom: -1ex;
        color: #70757a;
        padding-right: 2px;
    }

    &__col {
        border-right: 1px solid @grid-color;
        grid-row: 1 / span 24;
        grid-column: span 1;
    }

    &__filler-col {
        grid-row: 1 / -1;
        grid-column: 2;
        border-right: 1px solid @grid-color;
    }

    &__row {
        grid-column: 2 / -1;
        border-bottom: 1px solid @grid-color;
    }

    &__current-time {
        grid-column: 7;
        grid-row: 10;
        border-top: 2px solid @current-time-color;
        position: relative;
        top: calc(50% - 1px);
    }

    &__circle {
        width: 12px;
        height: 12px;
        border: 1px solid @current-time-color;
        border-radius: 50%;
        background: @current-time-color;
        position: relative;
        top: -6px;
        left: -6px;
    }
}
</style>

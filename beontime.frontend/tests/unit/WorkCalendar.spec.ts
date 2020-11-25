import { shallowMount, VueWrapper } from "@vue/test-utils";

import WorkCalendar from "@/views/WorkCalendar.vue";

import { User } from "@/services/userService";
import { enUS } from "date-fns/locale";

describe("WorkCalendar.vue", () => {
    let wrapper: VueWrapper<any>;
    beforeEach(() => {
        wrapper = shallowMount(WorkCalendar, {
            // data: () => ({
            //     currentDate: new Date(2020, 9, 14)
            // }),
            global: {
                provide: {
                    userService: {
                        user: {
                            data: {
                                displayName: "pozdro@z.gory"
                            },
                            preferences: {
                                locale: enUS
                            }
                        } as User
                    }
                }
            }
        });
    });

    afterEach(() => {
        wrapper.unmount();
    });

    it("renders 23 divs with .cal__row", () => {
        const rows = wrapper.findAll(".cal__row");
        expect(rows.length).toBe(23);
    });

    it("renders 23 divs with .cal__time", () => {
        const rows = wrapper.findAll(".cal__time");
        expect(rows.length).toBe(23);
    });

    it("renders 7 divs with .cal__col", () => {
        const rows = wrapper.findAll(".cal__col");
        expect(rows.length).toBe(7);
    });

    it("renders proper days name", async() => {
        console.log(wrapper.text());
        await wrapper.setData({ currentDate: new Date(2020, 9, 14) });
        expect(wrapper.text()).toContain("11th October");
    });
});

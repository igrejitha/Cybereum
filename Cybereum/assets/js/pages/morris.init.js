$(function () {
  "use strict";
  $("#morris-line-example").length &&
    Morris.Line({
      element: "morris-line-example",
      gridLineColor: "rgba(108, 120, 151, 0.1)",
      lineColors: ["#0db4d6", "#fb4d53"],
      xkey: "y",
      ykeys: ["a", "b"],
      hideHover: "auto",
      resize: !0,
      lineWidth: 2,
      labels: ["Series A", "Series B"],
      data: [
        { y: "2010", a: 110, b: 125 },
        { y: "2011", a: 170, b: 190 },
        { y: "2012", a: 120, b: 140 },
        { y: "2013", a: 80, b: 100 },
        { y: "2014", a: 110, b: 130 },
        { y: "2015", a: 90, b: 110 },
        { y: "2016", a: 120, b: 140 },
        { y: "2017", a: 110, b: 125 },
        { y: "2018", a: 170, b: 190 },
        { y: "2019", a: 120, b: 140 },
      ],
    }),
    $("#morris-bar-example").length &&
      Morris.Bar({
        element: "morris-bar-example",
        barColors: ["#eff2f7", "#11c46e"],
        data: [
          { y: "2010", a: 110, b: 125 },
          { y: "2011", a: 170, b: 190 },
          { y: "2012", a: 120, b: 140 },
          { y: "2013", a: 80, b: 100 },
          { y: "2014", a: 110, b: 130 },
          { y: "2015", a: 90, b: 110 },
          { y: "2016", a: 120, b: 140 },
          { y: "2017", a: 110, b: 125 },
          { y: "2018", a: 170, b: 190 },
          { y: "2019", a: 120, b: 140 },
        ],
        xkey: "y",
        ykeys: ["a", "b"],
        hideHover: "auto",
        gridLineColor: "rgba(108, 120, 151, 0.1)",
        resize: !0,
        barSizeRatio: 0.3,
        preUnits: "$",
        labels: ["Series A", "Series B"],
      }),
    $("#morris-area-example").length &&
      Morris.Area({
        element: "morris-area-example",
        lineColors: ["#3d8ef8", "#7c8a96"],
        data: [
          { y: "2013", a: 80, b: 100 },
          { y: "2014", a: 110, b: 130 },
          { y: "2015", a: 90, b: 110 },
          { y: "2016", a: 120, b: 140 },
          { y: "2017", a: 110, b: 125 },
          { y: "2018", a: 170, b: 190 },
          { y: "2019", a: 120, b: 140 },
        ],
        xkey: "y",
        ykeys: ["a", "b"],
        hideHover: "auto",
        gridLineColor: "rgba(108, 120, 151, 0.1)",
        resize: !0,
        fillOpacity: 0.4,
        lineWidth: 2,
        labels: ["Series A", "Series B"],
      }),
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    $("#morris-donut-example").length &&
      Morris.Donut({
        element: "morris-donut-example",
        resize: !0,
        colors: ["#0e9f80", "#d8a93b", "#cf523d"],
        data: [
          { label: "Completed", value: 50 },
          { label: "In-progress", value: 30 },
          { label: "Not started", value: 20 },
        ],
      }),
=======
=======
>>>>>>> Stashed changes
    //$("#morris-donut-example").length &&
    //  Morris.Donut({
    //    element: "morris-donut-example",
    //    resize: !0,
    //    colors: ["#0e9f80", "#d8a93b", "#cf523d"],
    //    data: [
    //      //{ label: "Completed", value: 50 },
    //      //{ label: "In-progress", value: 30 },
    //      //{ label: "Not started", value: 20 },
    //    ],
    //  }),
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
    $("#morris-stacked-example").length &&
      Morris.Bar({
        element: "morris-stacked-example",
        barColors: ["#3d8ef8", "#eff2f7"],
        data: [
          { y: "2010", a: 110, b: 125 },
          { y: "2011", a: 170, b: 190 },
          { y: "2012", a: 120, b: 140 },
          { y: "2013", a: 80, b: 100 },
          { y: "2014", a: 110, b: 130 },
          { y: "2015", a: 90, b: 110 },
          { y: "2016", a: 120, b: 140 },
          { y: "2017", a: 110, b: 125 },
          { y: "2018", a: 170, b: 190 },
          { y: "2019", a: 120, b: 140 },
        ],
        xkey: "y",
        ykeys: ["a", "b"],
        hideHover: "auto",
        gridLineColor: "rgba(108, 120, 151, 0.1)",
        resize: !0,
        barSizeRatio: 0.3,
        preUnits: "$",
        stacked: !0,
        labels: ["Series A", "Series B"],
      }),
    $("#morris-line-straight-example").length &&
      Morris.Line({
        element: "morris-line-straight-example",
        gridLineColor: "rgba(108, 120, 151, 0.1)",
        lineColors: ["#11c46e", "#7c8a96"],
        xkey: "y",
        ykeys: ["a", "b"],
        hideHover: "auto",
        resize: !0,
        lineWidth: 2,
        smooth: !1,
        labels: ["Series A", "Series B"],
        data: [
          { y: "2010", a: 110, b: 125 },
          { y: "2011", a: 170, b: 190 },
          { y: "2012", a: 120, b: 140 },
          { y: "2013", a: 80, b: 100 },
          { y: "2014", a: 110, b: 130 },
          { y: "2015", a: 90, b: 110 },
          { y: "2016", a: 120, b: 140 },
          { y: "2017", a: 110, b: 125 },
          { y: "2018", a: 170, b: 190 },
          { y: "2019", a: 120, b: 140 },
        ],
      });
});

$(function () {
    let table = $("table.pivot");
    let fixedRow = table.data("hrows");
    let fixedCol = table.data("hcols");
    let top = 0;
    for (let i = 0; i < fixedRow; i++) {
        let cell = $("table.pivot>thead>tr.headerRow>.cell.rh-" + i);
        if (cell.length === 0) break;
        $("table.pivot>thead>tr.headerRow>.rh-" + i).css({
            "position": "sticky",
            "top": top,
            "z-index": 1
        });
        top += cell.outerHeight();
    }
    $("table.pivot>thead>tr.headerRow>.measureTitle").css({
        "position": "sticky",
        "top": top,
        "z-index": 1
    });
    $("table.pivot>thead>tr.headerRow>.rowtitle").css({
        "position": "sticky",
        "top": top,
        "z-index": 1
    });

    let left = 0;
    for (let i = 0; i < fixedCol; i++) {
        let cell = $("table.pivot tr.dataRow .ch-" + i);
        if (cell.length === 0) break;
        $("table.pivot .ch-" + i).css({
            "position": "sticky",
            "left": left,
            "z-index": 2
        });
        left += cell.outerWidth();
    }
    $("table.pivot tr.dataRow .measureTitle").css({
        "position": "sticky",
        "left": left,
        "z-index": 2
    });
    $(".cornerHeader").css("z-index", 3);
    $(".coltitle ,.rowtitle").css("z-index", 3);

});

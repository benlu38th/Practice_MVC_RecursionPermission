$(document).ready(function () {
  // 隐藏所有具有 "submenu" 类的 <ul> 元素
  $(".submenu > ul").hide();

  // 選取所有具有 "submenu" 類別的元素中的直接子元素 "a"，
  // 並綁定 "click" 事件處理函式
  $(".submenu > a").click(function (e) {
    // 阻止默認的 <a> 元素點擊事件
    e.preventDefault();

    // 選取被點擊的 <a> 元素的父元素 <li>，存儲在變數 $li 中
    var $li = $(this).parent("li");

    // 選取被點擊的 <a> 元素的下一個 <ul> 元素，存儲在變數 $ul 中
    var $ul = $(this).next("ul");

    // 檢查 $li 是否具有 "open" 類別
    if ($li.hasClass("open")) {
      // 如果具有 "open" 類別，關閉相關聯的 <ul> 元素（向上滑動）
      $ul.slideUp(350);

      // 移除 "open" 類別
      $li.removeClass("open");
    } else {
      // 打開相關聯的 <ul> 元素（向下滑動）
      $ul.slideDown(350);
      // 添加 "open" 類別，標記為已打開狀態
      $li.addClass("open");
    }
  });
});

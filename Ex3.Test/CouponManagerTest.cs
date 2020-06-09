using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Ex3.Test
{
    [TestClass]
    public class CouponManagerTest
    {
        private ILogger _logger;
        private ICouponProvider _couponProvider;
        private CouponManager _manager;

        [TestInitialize]
        public void Initialise()
        {
            _logger = MockRepository.GenerateStrictMock<ILogger>();
            _couponProvider = MockRepository.GenerateStrictMock<ICouponProvider>();
            _manager = new CouponManager(_logger,_couponProvider);
        }

        [TestCleanup]
        public void Cleaup()
        {
            _logger.VerifyAllExpectations();
            _couponProvider.VerifyAllExpectations();
        }

        [TestMethod]
        public void CanRedeemCoupon_throw_argument_null_exception_when_evaluators_is_null()
        {

            var couponId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            IEnumerable<Func<Coupon, Guid, bool>> evaluators = null;

            var actual =Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                _manager.CanRedeemCoupon(couponId, userId, evaluators));

            Assert.IsNotNull(actual);
            Assert.AreEqual(nameof(evaluators), actual.Result.ParamName);

        }

        [TestMethod]
        public void CanRedeemCoupon_throw_key_not_found_exception_when_coupon_retrieve_null()
        {
            var couponId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            IEnumerable<Func<Coupon, Guid, bool>> evaluators = new List<Func<Coupon, Guid, bool>>()
            {
                (x, y) => { return true; },
                (x, y) => { return false; }
            };
        

            _couponProvider.Expect(m => m.Retrieve(couponId)).Return(null);

            var actual = Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _manager.CanRedeemCoupon(couponId, userId, evaluators));

            Assert.IsNotNull(actual);

        }

        [TestMethod]
        public void CanRedeemCoupon_return_true_when_evaluator_is_empty()
        {
            var couponId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            IEnumerable<Func<Coupon, Guid, bool>> evaluators = new List<Func<Coupon, Guid, bool>>();
            _couponProvider.Expect(m => m.Retrieve(couponId)).Return(Task.FromResult(new Coupon()));

            var actual = _manager.CanRedeemCoupon(couponId, userId, evaluators);

            Assert.AreEqual(true, actual.Result);
        }

        [TestMethod]
        public void CanRedeemCoupon_return_true_when_evaluator_has_true()
        {
            var couponId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            IEnumerable<Func<Coupon, Guid, bool>> evaluators = new List<Func<Coupon, Guid, bool>>()
            {
                (x, y) => { return true;},
                (x, y) => { return false;},

            };
            _couponProvider.Expect(m => m.Retrieve(couponId)).Return(Task.FromResult(new Coupon()));

            var actual = _manager.CanRedeemCoupon(couponId, userId, evaluators);

            Assert.AreEqual(true, actual.Result);

        }

        [TestMethod]
        public void CanRedeemCoupon_return_false_when_evaluator_has_all_false()
        {
            var couponId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            IEnumerable<Func<Coupon, Guid, bool>> evaluators = new List<Func<Coupon, Guid, bool>>()
            {
                (x, y) => { return false;},
                (x, y) => { return false;},

            };
            _couponProvider.Expect(m => m.Retrieve(couponId)).Return(Task.FromResult(new Coupon()));

            var actual = _manager.CanRedeemCoupon(couponId, userId, evaluators);

            Assert.AreEqual(false, actual.Result);

        }


    }
}
